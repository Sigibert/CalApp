using System;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace CalApp.Localization
{
    public enum AppLanguage { System, English, German }

    internal static class AppText
    {
        private static readonly ResourceManager Resources = new ResourceManager("CalApp.Localization.Strings", typeof(AppText).Assembly);
        // Capture the user's Windows display language before any manual choice is applied.
        private static readonly CultureInfo SystemCulture = CultureInfo.CurrentUICulture;
        internal static CultureInfo Culture { get; private set; } = Resolve(AppLanguage.System, SystemCulture);

        internal static CultureInfo Resolve(AppLanguage language, CultureInfo systemCulture)
        {
            if (!Enum.IsDefined(typeof(AppLanguage), language)) throw new ArgumentOutOfRangeException(nameof(language));
            return CultureInfo.GetCultureInfo(language == AppLanguage.German ||
                (language == AppLanguage.System && systemCulture.TwoLetterISOLanguageName == "de") ? "de-DE" : "en-GB");
        }

        internal static void UseLanguage(AppLanguage language) => Culture = Resolve(language, SystemCulture);
        internal static string Get(string key) => Resources.GetString(key, Culture)
            ?? throw new InvalidOperationException("Missing translation: " + key);
        internal static string Format(string key, params object[] values) => string.Format(Culture, Get(key), values);
        internal static string LanguageName(AppLanguage language) => language == AppLanguage.System
            ? Get("SystemLanguage") : language == AppLanguage.German ? "Deutsch" : "English";

        internal static void Translate(Control control)
        {
            string translated = Resources.GetString(control.Name, Culture);
            if (translated != null) control.Text = translated;
            if (control is ToolStrip strip)
                foreach (ToolStripItem item in strip.Items) Translate(item);
            foreach (Control child in control.Controls) Translate(child);
        }

        private static void Translate(ToolStripItem item)
        {
            string translated = Resources.GetString(item.Name, Culture);
            if (translated != null) item.Text = translated;
            if (item is ToolStripDropDownItem menu)
                foreach (ToolStripItem child in menu.DropDownItems) Translate(child);
        }
    }
}
