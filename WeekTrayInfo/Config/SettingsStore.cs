using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using WeekTrayInfo.Localization;

namespace WeekTrayInfo.Config
{
    public sealed class SettingsStore
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(AppSettings));
        private readonly string legacyRoot;
        private AppSettings current = new AppSettings();
        private bool preserveBackup;

        public string FilePath { get; }
        private string loadWarningKey;
        private string loadWarningDetail;
        public string LoadWarning => loadWarningKey == null ? null : AppText.Format(loadWarningKey, loadWarningDetail);
        public AppSettings Current => current.Clone();

        public SettingsStore(string filePath, string legacyRoot = null)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("A settings path is required.", nameof(filePath));
            FilePath = Path.GetFullPath(filePath);
            this.legacyRoot = legacyRoot;
        }

        public static SettingsStore CreateDefault()
        {
            string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WeekTrayInfo");
            return new SettingsStore(Path.Combine(root, "Preferences.xml"));
        }

        // Loading is explicit and always reads the file. There is no shared dirty flag.
        public AppSettings Load()
        {
            loadWarningKey = null;
            preserveBackup = false;
            current = new AppSettings();
            if (File.Exists(FilePath))
            {
                if (TryRead(FilePath, out AppSettings loaded))
                {
                    current = loaded;
                    return Current;
                }
                preserveBackup = true;
            }
            if (File.Exists(FilePath + ".bak"))
            {
                if (TryRead(FilePath + ".bak", out AppSettings backup))
                {
                    current = backup;
                    loadWarningKey = "BackupRestored";
                    return Current;
                }
            }
            // Do not silently substitute an old version for an existing broken current file.
            if (preserveBackup || LoadWarning != null) return Current;

            try
            {
                foreach (string candidate in GetLegacyFiles())
                {
                    if (!TryRead(candidate, out AppSettings migrated)) continue;
                    current = migrated;
                    loadWarningKey = null;
                    // Leave the original file untouched; persist to the stable path on Apply.
                    return Current;
                }
            }
            catch (Exception ex) when (IsStorageError(ex))
            {
                loadWarningKey = "LegacyReadFailed";
                loadWarningDetail = ex.Message;
            }
            return Current;
        }

        private IEnumerable<string> GetLegacyFiles()
        {
            if (legacyRoot == null || !Directory.Exists(legacyRoot)) return Enumerable.Empty<string>();
            return Directory.GetDirectories(legacyRoot)
                .Select(directory => new { Directory = directory, Version = ParseVersion(Path.GetFileName(directory)) })
                .Where(item => item.Version != null)
                .OrderByDescending(item => item.Version)
                .Select(item => Path.Combine(item.Directory, "Preferences.xml"))
                .Where(File.Exists).ToArray();
        }

        private static Version ParseVersion(string text) => Version.TryParse(text, out Version version) ? version : null;

        private bool TryRead(string path, out AppSettings settings)
        {
            try
            {
                settings = ReadSnapshot(path, true);
                return true;
            }
            catch (Exception ex) when (IsStorageError(ex) || ex is InvalidOperationException || ex is ArgumentException || ex is XmlException)
            {
                settings = null;
                loadWarningKey = "LoadFailed";
                loadWarningDetail = ex.Message;
                return false;
            }
        }

        internal static AppSettings ReadSnapshot(string path, bool normalizeLegacyIcons = false)
        {
            AppSettings settings;
            using (var reader = XmlReader.Create(path, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 1024 * 1024
            })) settings = (AppSettings)Serializer.Deserialize(reader);
            if (settings == null) throw new InvalidOperationException(AppText.Get("EmptyPreferences"));
            if (normalizeLegacyIcons && !settings.StandardIcon && !settings.FiscalWeekIcon && !settings.CalendarWeekIcon)
                settings.StandardIcon = true;
            settings.Validate();
            return settings;
        }

        // Write a complete file next to the destination before replacing it atomically.
        // Failure leaves both the original file and the in-memory settings unchanged.
        public void Save(AppSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            AppSettings saved = settings.Clone();
            saved.Validate();
            saved.DateOfChange = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            string temporary = FilePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    Serializer.Serialize(stream, saved);
                    stream.Flush(true);
                }
                if (File.Exists(FilePath))
                    File.Replace(temporary, FilePath, preserveBackup ? null : FilePath + ".bak");
                else
                    File.Move(temporary, FilePath);
                current = saved;
                preserveBackup = false;
                loadWarningKey = null;
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }

        internal static bool IsStorageError(Exception ex) => ex is IOException || ex is UnauthorizedAccessException || ex is SecurityException;
    }
}
