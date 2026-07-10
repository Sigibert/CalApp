using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CalApp.Preferences
{
    public static class Startup
    {
        public static void SetStartup(Boolean Checked)
        {
            // FIX: dispose the key (using), guard against a null key, and quote
            // the executable path so a path containing spaces (e.g. under
            // "C:\Program Files\...") is launched correctly by the Run entry.
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (rk == null)
                    return;

                if (Checked)
                    rk.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\"");
                else
                    rk.DeleteValue(Application.ProductName, false);
            }
        }
    }
}