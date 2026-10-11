using System;
using System.Threading;
using System.Windows.Forms;

namespace CalApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var mutex = new Mutex(true, "CalApp_SingleInstance", out bool isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    MessageBox.Show("Calendar Week Info is already running.", "Already Running",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var context = new TrayApplicationContext()) Application.Run(context);
            }
        }
    }
}
