using System;
using System.Drawing;
using System.Xml.Serialization;

namespace CalApp.Config
{
    [Serializable()]
    public sealed class ConfigDataObject
    {
        /// <summary>
        /// Returns the default configuration
        /// </summary>
        
        public ConfigDataObject()
        {
            this.StandardIcon = true;
            this.StandardIconColor = ColorTranslator.FromHtml("#005DA9");
            this.FiscalWeekIcon = true;
            this.FiscalWeekIconForeColor = ColorTranslator.FromHtml("#FFFF00");
            this.FiscalWeekIconBackColor = ColorTranslator.FromHtml("#0000FF");
            this.FiscalWeekIconForeColorAuto = true;
            this.CalendarWeekIcon = true;
            this.CalendarWeekIconForeColor = ColorTranslator.FromHtml("#0000FF");
            this.CalendarWeekIconBackColor = ColorTranslator.FromHtml("#FFFF00");
            this.CalendarWeekIconForeColorAuto = true;
            this.FirstDayOfFiscalWeek = DayOfWeek.Saturday;
            this.FiscalWeekRule = FiscalWeekRule.FirstFourDayWeek;
            this.FirstFiscalMonth = Month.October;
            this.StartWithWindows = true;
        }

        private String dateOfChange = "";
        /// <summary>
        /// Timestamp of when Preferences.xml was last saved. Stored in a
        /// culture-invariant format and set by ConfigMng.SaveConfig() right
        /// before serialization.
        /// FIX: was a fake property whose getter always returned DateTime.Now
        /// and whose setter discarded the value, so it never reflected the
        /// actual save time and round-tripped with a culture-dependent format.
        /// </summary>
        public String DateOfChange
        {
            get { return dateOfChange; }
            set { dateOfChange = value; }
        }

        /// <summary>
        /// The first day of a fiscal week
        /// </summary>
        private DayOfWeek firstDayOfFiscalWeek;
        public DayOfWeek FirstDayOfFiscalWeek
        {
            get { return firstDayOfFiscalWeek; }
            set { firstDayOfFiscalWeek = value; }
        }

        /// <summary>
        /// The number of days in a week that must be in the first fiscal month indicating the first fiscal week
        /// </summary>
        private FiscalWeekRule fiscalWeekRule;
        public FiscalWeekRule FiscalWeekRule
        {
            get { return fiscalWeekRule; }
            set { fiscalWeekRule = value; }
        }

        private Month firstFiscalMonth;
        /// <summary>
        /// The first month of the fiscal year
        /// </summary>
        public Month FirstFiscalMonth
        {
            get { return firstFiscalMonth; }
            set { firstFiscalMonth = value; }
        }

        private Boolean standardIcon;
        /// <summary>
        /// Activate or Deactivation of the Standard Icon
        /// </summary>
        public Boolean StandardIcon
        {
            get { return standardIcon; }
            set { standardIcon = value; }
        }

        private Color standardIconColor;
        /// <summary>
        /// The Method to set Color iconColor variable 
        /// </summary>
        [XmlIgnore]
        public Color StandardIconColor
        {
            get { return standardIconColor; }
            set { standardIconColor = value; }
        }
        /// <summary>
        /// The Serializeable Method of Color IconColor
        /// </summary>
        [XmlElement("StandardIconColor")]
        public String StandardIconColorHtml
        {
            get { return ColorTranslator.ToHtml(standardIconColor); }
            set { standardIconColor = ColorTranslator.FromHtml(value); }
        }

        private Boolean fiscalWeekIcon;
        /// <summary>
        /// Activate or Deactivation of the Fiscal Week Icon
        /// </summary>
        public Boolean FiscalWeekIcon
        {
            get { return fiscalWeekIcon; }
            set { fiscalWeekIcon = value; }
        }

        private Color fiscalWeekIconForeColor;
        /// <summary>
        /// The Method to set Color iconColor variable 
        /// </summary>
        [XmlIgnore]
        public Color FiscalWeekIconForeColor
        {
            get { return fiscalWeekIconForeColor; }
            set { fiscalWeekIconForeColor = value; }
        }
        /// <summary>
        /// The Serializeable Method of Color IconColor
        /// </summary>
        [XmlElement("FiscalWeekIconForeColor")]
        public String FiscalWeekIconForeColorHtml
        {
            get { return ColorTranslator.ToHtml(fiscalWeekIconForeColor); }
            set { fiscalWeekIconForeColor = ColorTranslator.FromHtml(value); }
        }

        private Boolean fiscalWeekIconForeColorAuto;
        /// <summary>
        /// When true, the fiscal week icon's text colour is auto-derived from
        /// its background (complement); when false, FiscalWeekIconForeColor is
        /// used as chosen by the user.
        /// </summary>
        public Boolean FiscalWeekIconForeColorAuto
        {
            get { return fiscalWeekIconForeColorAuto; }
            set { fiscalWeekIconForeColorAuto = value; }
        }

        private Color fiscalWeekIconBackColor;
        /// <summary>
        /// The Method to set Color iconColor variable 
        /// </summary>
        [XmlIgnore]
        public Color FiscalWeekIconBackColor
        {
            get { return fiscalWeekIconBackColor; }
            set { fiscalWeekIconBackColor = value; }
        }
        /// <summary>
        /// The Serializeable Method of Color IconColor
        /// </summary>
        [XmlElement("FiscalWeekIconBackColor")]
        public String FiscalWeekIconBackColorHtml
        {
            get { return ColorTranslator.ToHtml(fiscalWeekIconBackColor); }
            set { fiscalWeekIconBackColor = ColorTranslator.FromHtml(value); }
        }

        private Boolean calendarWeekIcon;
        /// <summary>
        /// Activate or Deactivation of the Calendar Week Icon
        /// </summary>
        public Boolean CalendarWeekIcon
        {
            get { return calendarWeekIcon; }
            set { calendarWeekIcon = value; }
        }

        private Color calendarWeekIconForeColor;
        /// <summary>
        /// The Method to set Color iconColor variable 
        /// </summary>
        [XmlIgnore]
        public Color CalendarWeekIconForeColor
        {
            get { return calendarWeekIconForeColor; }
            set { calendarWeekIconForeColor = value; }
        }
        /// <summary>
        /// The Serializeable Method of Color IconColor
        /// </summary>
        [XmlElement("CalendarWeekIconForeColor")]
        public String CalendarWeekIconForeColorHtml
        {
            get { return ColorTranslator.ToHtml(calendarWeekIconForeColor); }
            set { calendarWeekIconForeColor = ColorTranslator.FromHtml(value); }
        }

        private Boolean calendarWeekIconForeColorAuto;
        /// <summary>
        /// When true, the calendar week icon's text colour is auto-derived from
        /// its background (complement); when false, CalendarWeekIconForeColor is
        /// used as chosen by the user.
        /// </summary>
        public Boolean CalendarWeekIconForeColorAuto
        {
            get { return calendarWeekIconForeColorAuto; }
            set { calendarWeekIconForeColorAuto = value; }
        }

        private Color calendarWeekIconBackColor;
        /// <summary>
        /// The Method to set Color iconColor variable 
        /// </summary>
        [XmlIgnore]
        public Color CalendarWeekIconBackColor
        {
            get { return calendarWeekIconBackColor; }
            set { calendarWeekIconBackColor = value; }
        }
        /// <summary>
        /// The Serializeable Method of Color IconColor
        /// </summary>
        [XmlElement("CalendarWeekIconBackColor")]
        public String CalendarWeekIconBackColorHtml
        {
            get { return ColorTranslator.ToHtml(calendarWeekIconBackColor); }
            set { calendarWeekIconBackColor = ColorTranslator.FromHtml(value); }
        }

        private Boolean startWithWindows;
        /// <summary>
        /// Start Application with Windows
        /// </summary>
        public Boolean StartWithWindows
        {
            get { return startWithWindows; }
            set { startWithWindows = value; }
        }

    }
}