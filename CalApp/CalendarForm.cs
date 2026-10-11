using System;
using System.Windows.Forms;
using CalApp.DateFunctions;

namespace CalApp
{
    // The view displays calculated results and reports user requests to its owner.
    public partial class CalendarForm : Form
    {
        public event EventHandler DateSelectionChanged;
        public event EventHandler PreferencesRequested;
        public event EventHandler AboutRequested;
        public event EventHandler ExitRequested;
        public DateTime SelectedDate => dateCalendar.SelectionStart.Date;

        public CalendarForm()
        {
            InitializeComponent();
            dateCalendar.MaxSelectionCount = 1;
        }

        public void SelectDate(DateTime date) => dateCalendar.SetDate(date.Date);

        public void DisplayWeeks(WeekInfo weeks)
        {
            if (weeks == null) throw new ArgumentNullException(nameof(weeks));
            if (weeks.Date != SelectedDate) throw new ArgumentException("The week result must match the selected date.", nameof(weeks));
            calendarWeekLabel.Text = "Calendar Week: " + weeks.CalendarWeek;
            fiscalWeekLabel.Text = "Fiscal Week: " + weeks.Fiscal.Week;
            fiscalYearLabel.Text = "Fiscal Year: " + weeks.Fiscal.YearLabel;
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
