using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CalApp
{
    internal static class TrayIconIdentity
    {
        // Do not change these IDs between releases. A GUID is tied to its EXE path by Windows.
        internal static readonly Guid Standard = new Guid("78b166f3-eaa7-4d34-838f-446497e526b4");
        internal static readonly Guid FiscalWeek = new Guid("b1a0a514-e5d7-47ab-a0c0-a944d4f79c78");
        internal static readonly Guid CalendarWeek = new Guid("868ec947-6abc-416a-a9b2-86e94133a36b");

        internal static string InstalledExecutable => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Programs", "CalApp", "CalendarWeekInfo.exe");

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
