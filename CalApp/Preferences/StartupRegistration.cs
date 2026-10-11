using System;
using Microsoft.Win32;

namespace CalApp.Preferences
{
    public interface IStartupRegistration
    {
        string Command { get; set; }
    }

    public sealed class StartupRegistration : IStartupRegistration
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "Calendar Week Info";

        public string Command
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey))
                    return key?.GetValue(ValueName) as string;
            }
            set
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKey))
                {
                    if (key == null) throw new InvalidOperationException("The Windows startup registry key is unavailable.");
                    if (value == null) key.DeleteValue(ValueName, false);
                    else key.SetValue(ValueName, value, RegistryValueKind.String);
                }
            }
        }
    }
}
