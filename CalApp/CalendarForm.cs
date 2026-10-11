using System;
using System.Windows.Forms;
using CalApp.DateFunctions;
using CalApp.Localization;
using System.Drawing;

namespace CalApp
{
    // The view displays calculated results and reports user requests to its owner.
    public partial class CalendarForm : Form
    {
        public event EventHandler DateSelectionChanged;
        public event EventHandler PreferencesRequested;
        public event EventHandler AboutRequested;
        public event EventHandler ExitRequested;
        public event EventHandler TodayRequested;
        private readonly Button todayButton = new Button { Name = "todayButton", TabIndex = 8 };
        public DateTime SelectedDate => dateCalendar.SelectionStart.Date;

        public CalendarForm()
        {
            InitializeComponent();
            dateCalendar.MaxSelectionCount = 1;
            todayButton.Location = new Point(dateSelectionGroup.Left, dateSelectionGroup.Bottom + 6);
            todayButton.Click += TodayButton_Click;
            Controls.Add(todayButton);
            ApplyLanguage();
        }

        internal void ApplyLanguage()
        {
            AppText.Translate(this);
            todayButton.Width = Math.Max(75, TextRenderer.MeasureText(todayButton.Text, todayButton.Font).Width + 16);
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            if (keyData == Keys.Escape) { Hide(); return true; }
            if (keyData == (Keys.Control | Keys.T)) { TodayRequested?.Invoke(this, EventArgs.Empty); return true; }
            return base.ProcessCmdKey(ref message, keyData);
        }

        private void TodayButton_Click(object sender, EventArgs e) => TodayRequested?.Invoke(this, e);

        public void SelectDate(DateTime date) => dateCalendar.SetDate(date.Date);

        public void DisplayWeeks(WeekInfo weeks)
        {
            if (weeks == null) throw new ArgumentNullException(nameof(weeks));
            if (weeks.Date != SelectedDate) throw new ArgumentException("The week result must match the selected date.", nameof(weeks));
            calendarWeekLabel.Text = AppText.Format("CalendarWeek", weeks.CalendarWeek);
            fiscalWeekLabel.Text = AppText.Format("FiscalWeek", weeks.Fiscal.Week);
            fiscalYearLabel.Text = AppText.Format("FiscalYear", weeks.Fiscal.YearLabel);
        }

        public void RefreshToday(DateTime today) => dateCalendar.TodayDate = today.Date;

        public void ShowCalendar()
        {
            WindowState = FormWindowState.Normal;
            Show();
            BringToFront();
            Activate();
        }

        private void CalendarForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { Hide(); e.Cancel = true; }
        }

        private void CalendarForm_Move(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) Hide();
        }

        private void DateCalendar_DateChanged(object sender, DateRangeEventArgs e)
            => DateSelectionChanged?.Invoke(this, EventArgs.Empty);
        private void HideCalendarMenu_Click(object sender, EventArgs e) => Hide();
        private void PreferencesMenu_Click(object sender, EventArgs e) => PreferencesRequested?.Invoke(this, e);
        private void ExitMenu_Click(object sender, EventArgs e) => ExitRequested?.Invoke(this, e);
        private void AboutMenu_Click(object sender, EventArgs e) => AboutRequested?.Invoke(this, e);
    }
}
