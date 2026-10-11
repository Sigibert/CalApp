using System;
using System.Windows.Forms;
using CalApp.Config;
using CalApp.Preferences;

namespace CalApp
{
    internal sealed class TrayApplicationContext : ApplicationContext
    {
        private readonly CalendarForm calendarForm;

        public TrayApplicationContext() : this(new SettingsController(SettingsStore.CreateDefault(),
            new StartupRegistration(), Application.ExecutablePath)) { }

        internal TrayApplicationContext(SettingsController controller)
        {
            calendarForm = new CalendarForm(controller);
            calendarForm.EnsureHandleCreated();
            calendarForm.FormClosed += CalendarFormClosed;
            if (controller.LoadWarning != null)
                MessageBox.Show(controller.LoadWarning, "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void CalendarFormClosed(object sender, FormClosedEventArgs e) => ExitThread();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                calendarForm.FormClosed -= CalendarFormClosed;
                calendarForm.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
