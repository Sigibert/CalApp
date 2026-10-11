using System;
using System.Drawing;
using System.Xml.Serialization;

namespace CalApp.Config
{
    // Keep the old XML contract so existing Preferences.xml files remain readable.
    [XmlRoot("ConfigDataObject")]
    public sealed class AppSettings
    {
        public string DateOfChange { get; set; } = "";
        public DayOfWeek FirstDayOfFiscalWeek { get; set; } = DayOfWeek.Saturday;
        public FiscalWeekRule FiscalWeekRule { get; set; } = FiscalWeekRule.FirstFourDayWeek;
        public Month FirstFiscalMonth { get; set; } = Month.October;
        public bool StandardIcon { get; set; } = true;
        public bool FiscalWeekIcon { get; set; } = true;
        public bool CalendarWeekIcon { get; set; } = true;
        public bool FiscalWeekIconForeColorAuto { get; set; } = true;
        public bool CalendarWeekIconForeColorAuto { get; set; } = true;
        public bool StartWithWindows { get; set; } = true;

        [XmlIgnore] public Color StandardIconColor { get; set; } = Color.FromArgb(0, 93, 169);
        [XmlIgnore] public Color FiscalWeekIconForeColor { get; set; } = Color.Yellow;
        [XmlIgnore] public Color FiscalWeekIconBackColor { get; set; } = Color.Blue;
        [XmlIgnore] public Color CalendarWeekIconForeColor { get; set; } = Color.Blue;
        [XmlIgnore] public Color CalendarWeekIconBackColor { get; set; } = Color.Yellow;

        [XmlElement("StandardIconColor")]
        public string StandardIconColorHtml { get => ColorTranslator.ToHtml(StandardIconColor); set => StandardIconColor = ColorTranslator.FromHtml(value); }
        [XmlElement("FiscalWeekIconForeColor")]
        public string FiscalWeekIconForeColorHtml { get => ColorTranslator.ToHtml(FiscalWeekIconForeColor); set => FiscalWeekIconForeColor = ColorTranslator.FromHtml(value); }
        [XmlElement("FiscalWeekIconBackColor")]
        public string FiscalWeekIconBackColorHtml { get => ColorTranslator.ToHtml(FiscalWeekIconBackColor); set => FiscalWeekIconBackColor = ColorTranslator.FromHtml(value); }
        [XmlElement("CalendarWeekIconForeColor")]
        public string CalendarWeekIconForeColorHtml { get => ColorTranslator.ToHtml(CalendarWeekIconForeColor); set => CalendarWeekIconForeColor = ColorTranslator.FromHtml(value); }
        [XmlElement("CalendarWeekIconBackColor")]
        public string CalendarWeekIconBackColorHtml { get => ColorTranslator.ToHtml(CalendarWeekIconBackColor); set => CalendarWeekIconBackColor = ColorTranslator.FromHtml(value); }

        public AppSettings Clone() => (AppSettings)MemberwiseClone();

        public void Validate()
        {
            if (!Enum.IsDefined(typeof(DayOfWeek), FirstDayOfFiscalWeek)
                || !Enum.IsDefined(typeof(FiscalWeekRule), FiscalWeekRule)
                || !Enum.IsDefined(typeof(Month), FirstFiscalMonth))
                throw new ArgumentException("The fiscal calendar settings are invalid.");
            if (!StandardIcon && !FiscalWeekIcon && !CalendarWeekIcon)
                throw new ArgumentException("At least one tray icon must remain visible.");
            if (StandardIconColor.IsEmpty || FiscalWeekIconForeColor.IsEmpty || FiscalWeekIconBackColor.IsEmpty
                || CalendarWeekIconForeColor.IsEmpty || CalendarWeekIconBackColor.IsEmpty)
                throw new ArgumentException("An icon color is missing.");
        }
    }
}
