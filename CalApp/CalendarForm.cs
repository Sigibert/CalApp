using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.DateFunctions;
using CalApp.Preferences;

namespace CalApp
{
    public partial class CalendarForm : Form
    {
        private readonly Timer refreshTimer = new Timer();
        private readonly Icon calendarIcon = (Icon)Resource.Calendar_Icon.Clone();
        private readonly SettingsController settingsController;
        private DateTime lastRefreshHour;

        public CalendarForm() : this(new SettingsController(SettingsStore.CreateDefault(),
            new StartupRegistration(), Application.ExecutablePath)) { }

        public CalendarForm(SettingsController settingsController)
        {
            this.settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));
            InitializeComponent();
            dateCalendar.MaxSelectionCount = 1;
            UpdateIcons(DateTime.Now);
            settingsController.Saved += SettingsSaved;
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Interval = RefreshSchedule.GetInterval(DateTime.Now);
            refreshTimer.Start();
        }

        public void EnsureHandleCreated()
        {
            if (!IsHandleCreated) { var unused = Handle; }
        }

        private void CalendarForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { Hide(); e.Cancel = true; }
        }

        private void CalendarForm_Move(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) Hide();
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e) => ShowCalendar();
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) => Hide();
        private void PreferencesToolStripMenuItem_Click(object sender, EventArgs e) => ShowPreferences();
        private void PreferencesToolStripMenuItem1_Click(object sender, EventArgs e) => ShowPreferences();
        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e) => Application.Exit();
        private void CloseToolStripMenuItem1_Click(object sender, EventArgs e) => Hide();

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (Visible) Hide(); else ShowCalendar();
        }

        public void ShowCalendar()
        {
            DateTime now = DateTime.Now;
            UpdateIcons(now);
            dateCalendar.TodayDate = now.Date;
            dateCalendar.SetDate(now.Date);
            UpdateDateLabels(now.Date);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            Show();
            BringToFront();
            Activate();
        }

        private void UpdateDateLabels(DateTime date)
        {
            AppSettings settings = settingsController.Settings;
            FiscalDate fiscal = FiscalCalendar.GetDate(date, settings.FirstDayOfFiscalWeek,
                settings.FiscalWeekRule, settings.FirstFiscalMonth);
            calendarWeekLabel.Text = "Calendar Week: " + IsoCalendar.GetWeek(date);
            fiscalWeekLabel.Text = "Fiscal Week: " + fiscal.Week;
            fiscalYearLabel.Text = "Fiscal Year: " + fiscal.YearLabel;
        }

        private void DateCalendar_DateChanged(object sender, DateRangeEventArgs e)
            => UpdateDateLabels(dateCalendar.SelectionStart);

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (RefreshSchedule.GetHour(now) != lastRefreshHour) UpdateIcons(now);
            dateCalendar.TodayDate = now.Date;
            refreshTimer.Interval = RefreshSchedule.GetInterval(now);
        }

        private void SettingsSaved(object sender, EventArgs e)
        {
            UpdateIcons(DateTime.Now);
            UpdateDateLabels(dateCalendar.SelectionStart);
        }

        private void ShowPreferences()
        {
            using (var dialog = new PreferencesForm(settingsController)) dialog.ShowDialog(this);
        }

        private void UpdateIcons(DateTime now)
        {
            AppSettings settings = settingsController.Settings;
            FiscalDate fiscal = FiscalCalendar.GetDate(now, settings.FirstDayOfFiscalWeek,
                settings.FiscalWeekRule, settings.FirstFiscalMonth);
            Color fiscalText = settings.FiscalWeekIconForeColorAuto
                ? IconColors.GetTextColor(settings.FiscalWeekIconBackColor) : settings.FiscalWeekIconForeColor;
            Color calendarText = settings.CalendarWeekIconForeColorAuto
                ? IconColors.GetTextColor(settings.CalendarWeekIconBackColor) : settings.CalendarWeekIconForeColor;
            ReplaceIcon(fiscalWeekTrayIcon, IconFactory.CreateText(fiscal.Week.ToString(CultureInfo.InvariantCulture),
                settings.FiscalWeekIconBackColor, fiscalText));
            ReplaceIcon(calendarWeekTrayIcon, IconFactory.CreateText(IsoCalendar.GetWeek(now).ToString(CultureInfo.InvariantCulture),
                settings.CalendarWeekIconBackColor, calendarText));
            ReplaceIcon(standardTrayIcon, IconFactory.Recolor(calendarIcon, settings.StandardIconColor));
            fiscalWeekTrayIcon.Text = "Fiscal week " + fiscal.Week + " (" + fiscal.YearLabel + ")";
            calendarWeekTrayIcon.Text = "Calendar week " + IsoCalendar.GetWeek(now);
            standardTrayIcon.Text = "Calendar Week Info";
            standardTrayIcon.Visible = settings.StandardIcon;
            fiscalWeekTrayIcon.Visible = settings.FiscalWeekIcon;
            calendarWeekTrayIcon.Visible = settings.CalendarWeekIcon;
            lastRefreshHour = RefreshSchedule.GetHour(now);
        }

        private static void ReplaceIcon(NotifyIcon tray, Icon replacement)
        {
            Icon previous = tray.Icon;
            try { tray.Icon = replacement; }
            catch { replacement.Dispose(); throw; }
            previous?.Dispose();
        }

        private void ReleaseTrayResources()
        {
            refreshTimer.Dispose();
            settingsController.Saved -= SettingsSaved;
            foreach (NotifyIcon tray in new[] { standardTrayIcon, fiscalWeekTrayIcon, calendarWeekTrayIcon })
            {
                if (tray == null) continue;
                tray.Visible = false;
                Icon previous = tray.Icon;
                tray.Icon = null;
                previous?.Dispose();
            }
            calendarIcon.Dispose();
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new AboutForm()) dialog.ShowDialog(this);
        }
    }
}
