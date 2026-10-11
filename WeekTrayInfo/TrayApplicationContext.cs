using System;
using System.Windows.Forms;
using WeekTrayInfo.Config;
using WeekTrayInfo.DateFunctions;
using WeekTrayInfo.Preferences;
using WeekTrayInfo.Localization;

namespace WeekTrayInfo
{
    // The tray application owns lifetime, settings, refresh, and lazy UI creation.
    internal sealed class TrayApplicationContext : ApplicationContext
    {
        private readonly SettingsController settingsController;
        private readonly Func<DateTime> clock;
        private readonly Timer refreshTimer = new Timer();
        private readonly ApplicationLifetimeWindow lifetimeWindow = new ApplicationLifetimeWindow();
        private readonly TrayIcons trayIcons;
        private CalendarForm calendarForm;
        private Form activeDialog;
        private DateTime lastRefreshHour;
        private bool disposed;

        public TrayApplicationContext() : this(new SettingsController(SettingsStore.CreateDefault(),
            new StartupRegistration(), Application.ExecutablePath), persistentIcons: true) { }

        internal TrayApplicationContext(SettingsController controller, Func<DateTime> clock = null, bool persistentIcons = false)
        {
            settingsController = controller ?? throw new ArgumentNullException(nameof(controller));
            AppText.UseLanguage(controller.Settings.Language);
            trayIcons = new TrayIcons(persistentIcons);
            this.clock = clock ?? (() => DateTime.Now);
            lifetimeWindow.FormClosed += LifetimeWindowClosed;
            settingsController.Saved += SettingsSaved;
            trayIcons.CalendarToggleRequested += ToggleCalendar;
            trayIcons.CalendarShowRequested += ShowCalendar;
            trayIcons.CalendarHideRequested += HideCalendar;
            trayIcons.PreferencesRequested += ShowPreferences;
            trayIcons.ExitRequested += ExitApplication;
            trayIcons.UpdatesRequested += ShowUpdates;
            DateTime now = this.clock();
            UpdateIcons(now);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Interval = RefreshSchedule.GetInterval(now);
            refreshTimer.Start();
            if (controller.LoadWarning != null)
                MessageBox.Show(lifetimeWindow, controller.LoadWarning, AppText.Get("PreferencesForm"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        internal CalendarForm GetCalendar()
        {
            if (disposed) throw new ObjectDisposedException(nameof(TrayApplicationContext));
            if (calendarForm == null)
            {
                calendarForm = new CalendarForm();
                calendarForm.DateSelectionChanged += CalendarDateChanged;
                calendarForm.PreferencesRequested += ShowPreferences;
                calendarForm.AboutRequested += ShowAbout;
                calendarForm.ExitRequested += ExitApplication;
                calendarForm.TodayRequested += SelectToday;
                UpdateSelectedDate();
            }
            return calendarForm;
        }

        private WeekInfo CalculateWeeks(DateTime date)
        {
            AppSettings settings = settingsController.Settings;
            return WeekCalculator.GetWeeks(date, settings.FirstDayOfFiscalWeek,
                settings.FiscalWeekRule, settings.FirstFiscalMonth);
        }

        private void UpdateSelectedDate()
        {
            if (calendarForm != null) calendarForm.DisplayWeeks(CalculateWeeks(calendarForm.SelectedDate));
        }

        private void UpdateIcons(DateTime now)
        {
            AppSettings settings = settingsController.Settings;
            WeekInfo weeks = WeekCalculator.GetWeeks(now, settings.FirstDayOfFiscalWeek,
                settings.FiscalWeekRule, settings.FirstFiscalMonth);
            trayIcons.Update(settings, weeks);
            lastRefreshHour = RefreshSchedule.GetHour(now);
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = clock();
            if (RefreshSchedule.GetHour(now) != lastRefreshHour) UpdateIcons(now);
            trayIcons.RetryRegistration();
            calendarForm?.RefreshToday(now);
            refreshTimer.Interval = RefreshSchedule.GetInterval(now);
        }

        private void SettingsSaved(object sender, EventArgs e)
        {
            AppText.UseLanguage(settingsController.Settings.Language);
            calendarForm?.ApplyLanguage();
            UpdateIcons(clock());
            UpdateSelectedDate();
        }

        private void CalendarDateChanged(object sender, EventArgs e) => UpdateSelectedDate();

        private void SelectToday(object sender, EventArgs e)
        {
            DateTime now = clock();
            calendarForm.RefreshToday(now);
            calendarForm.SelectDate(now);
            UpdateSelectedDate();
        }

        private void ToggleCalendar(object sender, EventArgs e)
        {
            if (ActivateDialog()) return;
            if (calendarForm != null && calendarForm.Visible) calendarForm.Hide();
            else ShowCalendar(sender, e);
        }

        private void ShowCalendar(object sender, EventArgs e)
        {
            if (ActivateDialog()) return;
            DateTime now = clock();
            UpdateIcons(now);
            CalendarForm view = GetCalendar();
            view.RefreshToday(now);
            view.SelectDate(now);
            UpdateSelectedDate();
            view.ShowCalendar();
        }

        private void HideCalendar(object sender, EventArgs e)
        {
            if (!ActivateDialog()) calendarForm?.Hide();
        }

        private void ShowPreferences(object sender, EventArgs e)
            => ShowDialog(() => new PreferencesForm(settingsController, clock));
        private void ShowAbout(object sender, EventArgs e) => ShowDialog(() => new AboutForm());
        private void ShowUpdates(object sender, EventArgs e) => ShowDialog(() => new UpdateForm());

        private bool ActivateDialog()
        {
            if (activeDialog == null) return false;
            activeDialog.BringToFront();
            activeDialog.Activate();
            return true;
        }

        private void ShowDialog(Func<Form> createDialog)
        {
            if (ActivateDialog()) return;
            using (Form dialog = createDialog())
            {
                activeDialog = dialog;
                try { dialog.ShowDialog(calendarForm != null && calendarForm.Visible ? (Form)calendarForm : lifetimeWindow); }
                finally { activeDialog = null; }
            }
        }

        private void ExitApplication(object sender, EventArgs e) => Application.Exit();
        private void LifetimeWindowClosed(object sender, FormClosedEventArgs e) => ExitThread();

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                refreshTimer.Dispose();
                settingsController.Saved -= SettingsSaved;
                lifetimeWindow.FormClosed -= LifetimeWindowClosed;
                if (calendarForm != null)
                {
                    calendarForm.DateSelectionChanged -= CalendarDateChanged;
                    calendarForm.PreferencesRequested -= ShowPreferences;
                    calendarForm.AboutRequested -= ShowAbout;
                    calendarForm.ExitRequested -= ExitApplication;
                    calendarForm.TodayRequested -= SelectToday;
                    calendarForm.Dispose();
                }
                trayIcons.Dispose();
                lifetimeWindow.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
