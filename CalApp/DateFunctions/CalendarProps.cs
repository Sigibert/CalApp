using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CalApp.DateFunctions
{
    class CalendarProps
    {
        public static string CalendarWeek(DateTime dateTime)
        {
            // FIX: Calendar.GetWeekOfYear with FirstFourDayWeek does NOT match
            // ISO 8601 in boundary weeks (it can return 53 where ISO says 1,
            // and vice versa). ISOWeek isn't available on .NET Framework 4.7.2,
            // so we apply Microsoft's documented correction: any day that falls
            // Mon-Wed belongs to the ISO week containing that week's Thursday,
            // so we shift such dates forward to their Thursday before asking
            // GetWeekOfYear. This yields correct ISO 8601 week numbers.
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;

            DateTime target = dateTime.Date;
            DayOfWeek day = calendar.GetDayOfWeek(target);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                target = target.AddDays(3);

            int calendarWeek = calendar.GetWeekOfYear(
                target, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return calendarWeek.ToString();
        }
    }
}
