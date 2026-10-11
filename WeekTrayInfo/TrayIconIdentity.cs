using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WeekTrayInfo
{
    internal static class TrayIconIdentity
    {
        // Do not change these IDs between releases. A GUID is tied to its EXE path by Windows.
        internal static readonly Guid Standard = new Guid("f1c91595-65d7-494d-8510-a926cf636dcd");
        internal static readonly Guid FiscalWeek = new Guid("d699081f-2c78-4adb-afdc-b8c0cd352f4f");
        internal static readonly Guid CalendarWeek = new Guid("9ce18eab-ee27-4580-980d-fbd88e5e5a80");

        internal static string InstalledExecutable => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Programs", "WeekTrayInfo", "WeekTrayInfo.exe");

        internal static Guid ForExecutable(Guid identity, string executablePath)
        {
            string path = Path.GetFullPath(executablePath);
            if (string.Equals(path, InstalledExecutable, StringComparison.OrdinalIgnoreCase)) return identity;
            // Portable/development copies must not claim the installed application's GUID.
            using (var hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(identity + "|" + path.ToUpperInvariant()));
                byte[] guid = new byte[16];
                Array.Copy(bytes, guid, guid.Length);
                return new Guid(guid);
            }
        }
    }
}
