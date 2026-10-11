using System;
using System.IO;
using CalApp.Localization;

namespace CalApp.Config
{
    internal static class SettingsTransfer
    {
        internal static AppSettings Import(string path) => SettingsStore.ReadSnapshot(path);

        internal static void Export(string path, AppSettings settings, string activeSettingsPath)
        {
            string target = Path.GetFullPath(path);
            string active = Path.GetFullPath(activeSettingsPath);
            // Export must never bypass Apply or overwrite the active recovery file.
            if (string.Equals(target, active, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(target, active + ".bak", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(target + ".bak", active, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(AppText.Get("ExportActiveFile"));
            new SettingsStore(target).Save(settings);
        }
    }
}
