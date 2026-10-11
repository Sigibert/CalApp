using System;
using System.Globalization;

namespace WeekTrayInfo.DateFunctions
{
    public static class IsoCalendar
    {
        public static int GetWeek(DateTime date)
        {
            // Moving Monday-Wednesday forward avoids GetWeekOfYear's year-end ambiguity.
            DayOfWeek day = date.DayOfWeek;
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday && date.Year < 9999)
                date = date.AddDays(3);

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
