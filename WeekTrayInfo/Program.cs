using System;
using System.Threading;
using System.Windows.Forms;
using WeekTrayInfo.Config;
using WeekTrayInfo.Preferences;
using WeekTrayInfo.Localization;

namespace WeekTrayInfo
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--sync-startup")
            {
                try
                {
                    new SettingsController(SettingsStore.CreateDefault(), new StartupRegistration(),
                        Application.ExecutablePath).SynchronizeStartup();
                    return 0;
                }
                catch (Exception ex) when (SettingsStore.IsStorageError(ex) || ex is InvalidOperationException)
                {
                    return 1;
                }
            }
            using (var mutex = new Mutex(true, "WeekTrayInfo_SingleInstance", out bool isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    AppText.UseLanguage(SettingsStore.CreateDefault().Load().Language);
                    MessageBox.Show(AppText.Get("AlreadyRunning"), AppText.Get("AlreadyRunningTitle"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return 0;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var context = new TrayApplicationContext()) Application.Run(context);
            }
            return 0;
        }
    }
}
