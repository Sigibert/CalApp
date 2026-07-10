using System;
using System.Threading;
using System.Windows.Forms;

namespace CalApp
{
    static class Program
    {
        // FIX: static field keeps Form1 reachable by the GC for the
        // entire application lifetime. A local variable could be collected
        // after the constructor returns.
        private static Form1 mainForm;

        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(true, "CalApp_SingleInstance", out bool isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    MessageBox.Show(
                        "Calendar Week Info is already running.",
                        "Already Running",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                mainForm = new Form1();

                // FIX: the form is never shown at startup, so its window
                // handle would otherwise not be created until the user opens
                // it the first time. Until the handle exists, IconTimerEvent's
                // this.Invoke(...) is skipped (guarded by IsHandleCreated),
                // meaning the tray icons would never refresh on their own at
                // the top of the hour / at midnight. Force handle creation now.
                mainForm.EnsureHandleCreated();

                Application.Run(new ApplicationContext());
            }
        }
    }
}