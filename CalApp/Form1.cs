using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using CalApp.Config;
using CalApp.DateFunctions;

namespace CalApp
{
    public partial class Form1 : Form
    {
        private readonly System.Timers.Timer iconTimer = new System.Timers.Timer();
        public static readonly Icon icon = Resource.Calendar_Icon;
        private ConfigMng configMng = new ConfigMng();

        public Form1()
        {
            InitializeComponent();

            configMng.LoadConfig();
            contextMenuStrip1.Hide();

            // FIX: updated to MillisecondsLeftTillNextUpdate so icons
            // also refresh at midnight, not just on the hour.
            iconTimer.Interval = TimerCalculation.MillisecondsLeftTillNextUpdate();
            iconTimer.Elapsed += IconTimerEvent;
            iconTimer.Start();

            SetIcons();
        }

        /// <summary>
        /// Forces the underlying window handle to be created without showing
        /// the form. Required so IconTimerEvent can marshal to the UI thread
        /// via Invoke even before the user opens the window for the first time.
        /// </summary>
        public void EnsureHandleCreated()
        {
            if (!this.IsHandleCreated)
            {
                // Accessing the Handle property forces its creation.
                var unused = this.Handle;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // FIX: only intercept the user closing the window (X button /
            // Close menu) — hide instead of exit. Let Windows shutdown/log-off
            // and explicit application exit (EndOfApp -> Application.Exit)
            // proceed normally so we don't block or delay them.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void Form1_Paint(object sender, PaintEventArgs e) { }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e) => ShowForm();
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => EndOfApp();
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) => this.Hide();
        private void PreferencesToolStripMenuItem_Click(object sender, EventArgs e) => Preferences();
        private void PreferencesToolStripMenuItem1_Click(object sender, EventArgs e) => Preferences();
        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e) => EndOfApp();
        private void CloseToolStripMenuItem1_Click(object sender, EventArgs e) => this.Hide();

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // FIX: removed the dead Activate() on an about-to-be-hidden form
                // and the redundant WriteDateData call — ShowForm now populates
                // the labels itself on every show path.
                if (this.Visible)
                    this.Hide();
                else
                    ShowForm();
            }
        }

        public void EndOfApp()
        {
            // FIX: don't call this.Close() here — programmatic Close() reports
            // CloseReason.UserClosing, which Form1_FormClosing cancels (hide
            // instead of exit), so it never actually closed. Application.Exit()
            // tears down all forms with CloseReason.ApplicationExitCall, which
            // Form1_FormClosing lets through, giving a clean shutdown.
            Application.Exit();
        }

        public void ShowForm()
        {
            // FIX: populate the labels here so every entry point (tray click and
            // the "Show" context-menu item) refreshes them. Previously only the
            // tray-click path called WriteDateData, so opening via the menu could
            // leave the labels showing their designer placeholder text.
            monthCalendar1.SelectionStart = DateTime.Now;
            WriteDateData(DateTime.Now);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Activate();
        }

        public void WriteDateData(DateTime dateTime)
        {
            configMng.LoadConfig();

            string calendarWeek = CalendarProps.CalendarWeek(dateTime);
            string fiscalWeek = FiscalCalendarProps.FiscalWeek(dateTime,
                configMng.ConfigDataObject.FirstDayOfFiscalWeek,
                configMng.ConfigDataObject.FiscalWeekRule,
                configMng.ConfigDataObject.FirstFiscalMonth);
            string fiscalYear = FiscalCalendarProps.FiscalYear(dateTime,
                configMng.ConfigDataObject.FirstDayOfFiscalWeek,
                configMng.ConfigDataObject.FiscalWeekRule,
                configMng.ConfigDataObject.FirstFiscalMonth);

            this.label1.Text = "Calendar Week: " + calendarWeek;
            this.label2.Text = "Fiscal Week: " + fiscalWeek;
            this.label3.Text = "Fiscal Year: " + fiscalYear;
        }

        private void MonthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            WriteDateData(monthCalendar1.SelectionStart.Date);
        }

        public void IconTimerEvent(object sender, ElapsedEventArgs e)
        {
            // FIX: System.Timers.Timer fires on a ThreadPool thread.
            // All UI access marshalled to the UI thread via Invoke.
            // IsHandleCreated guard prevents crashes during shutdown.
            if (this.IsHandleCreated)
            {
                this.Invoke(new Action(() =>
                {
                    SetIcons();
                    // FIX: updated to MillisecondsLeftTillNextUpdate.
                    iconTimer.Interval = TimerCalculation.MillisecondsLeftTillNextUpdate();
                }));
            }
        }

        public void Form2_SaveEvent(object objSender, EventArgs e)
        {
            // FIX: removed Console.WriteLine debug call.
            SetIcons();
            WriteDateData(DateTime.Now);
        }

        private void Preferences()
        {
            Form2 preferencesForm = new Form2();
            preferencesForm.ApplyEvent += Form2_SaveEvent;
            preferencesForm.ShowDialog();
        }

        private void SetIcons()
        {
            configMng.LoadConfig();
            notifyIconStandard.Visible = configMng.ConfigDataObject.StandardIcon;
            notifyIconFiscalWeek.Visible = configMng.ConfigDataObject.FiscalWeekIcon;
            notifyIconCalendarWeek.Visible = configMng.ConfigDataObject.CalendarWeekIcon;

            string fW = FiscalCalendarProps.FiscalWeek(DateTime.Now,
                configMng.ConfigDataObject.FirstDayOfFiscalWeek,
                configMng.ConfigDataObject.FiscalWeekRule,
                configMng.ConfigDataObject.FirstFiscalMonth);
            string cW = CalendarProps.CalendarWeek(DateTime.Now);

            // FIX: dispose previous icons before replacing to prevent
            // GDI handle accumulation over repeated hourly updates.
            notifyIconFiscalWeek.Icon?.Dispose();
            notifyIconFiscalWeek.Icon = IconFunctions.CreateTextIcon(fW,
                configMng.ConfigDataObject.FiscalWeekIconBackColor,
                configMng.ConfigDataObject.FiscalWeekIconForeColor);

            notifyIconCalendarWeek.Icon?.Dispose();
            notifyIconCalendarWeek.Icon = IconFunctions.CreateTextIcon(cW,
                configMng.ConfigDataObject.CalendarWeekIconBackColor,
                configMng.ConfigDataObject.CalendarWeekIconForeColor);

            notifyIconStandard.Icon?.Dispose();
            notifyIconStandard.Icon = IconFunctions.ChangeToColor(icon,
                configMng.ConfigDataObject.StandardIconColor);

            this.Refresh();
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form3().ShowDialog();
        }
    }
}