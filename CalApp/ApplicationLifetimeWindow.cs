using System.Windows.Forms;

namespace CalApp
{
    // A lightweight, permanently hidden owner receives shutdown/logoff messages.
    // Unlike CalendarForm, it creates no calendar controls or application UI.
    internal sealed class ApplicationLifetimeWindow : Form
    {
        public ApplicationLifetimeWindow()
        {
            Text = "Calendar Week Info";
            ShowInTaskbar = false;
            var unused = Handle;
        }

        protected override void SetVisibleCore(bool value) => base.SetVisibleCore(false);
    }
}
