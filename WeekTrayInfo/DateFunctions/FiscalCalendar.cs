using System;
using System.Globalization;

namespace WeekTrayInfo.DateFunctions
{
    public sealed class FiscalDate
    {
        public int Week { get; }
        public int StartYear { get; }
        public string YearLabel => (StartYear % 100).ToString("00", CultureInfo.InvariantCulture)
            + "/" + ((StartYear + 1) % 100).ToString("00", CultureInfo.InvariantCulture);

        internal FiscalDate(int week, int startYear)
        {
            Week = week;
            StartYear = startYear;
        }
    }

    public static class FiscalCalendar
    {
        public static FiscalDate GetDate(DateTime date, DayOfWeek firstDay,
            FiscalWeekRule rule, Month firstMonth)
        {
            Validate(firstDay, rule, firstMonth);
            date = date.Date;
            int year = date.Year;
            DateTime start = GetYearStart(year, firstDay, rule, firstMonth);
            if (date < start)
            {
                if (year == 1) throw new ArgumentOutOfRangeException(nameof(date));
                start = GetYearStart(--year, firstDay, rule, firstMonth);
            }
            else if (year < 9999)
            {
                DateTime nextStart = GetYearStart(year + 1, firstDay, rule, firstMonth);
                if (date >= nextStart)
                {
                    start = nextStart;
                    year++;
                }
            }
            return new FiscalDate((date - start).Days / 7 + 1, year);
        }

        public static DateTime GetYearStart(int year, DayOfWeek firstDay,
            FiscalWeekRule rule, Month firstMonth)
        {
            Validate(firstDay, rule, firstMonth);
            DateTime first = new DateTime(year, (int)firstMonth, 1);
            int precedingDays = ((int)first.DayOfWeek - (int)firstDay + 7) % 7;
            int shift = 7 - precedingDays >= (int)rule ? -precedingDays : 7 - precedingDays;
            if (first.Ticks + (long)shift * TimeSpan.TicksPerDay < DateTime.MinValue.Ticks)
                throw new ArgumentOutOfRangeException(nameof(year));
            return first.AddDays(shift);
        }

        private static void Validate(DayOfWeek firstDay, FiscalWeekRule rule, Month firstMonth)
        {
            if (!Enum.IsDefined(typeof(DayOfWeek), firstDay)) throw new ArgumentOutOfRangeException(nameof(firstDay));
            if (!Enum.IsDefined(typeof(FiscalWeekRule), rule)) throw new ArgumentOutOfRangeException(nameof(rule));
            if (!Enum.IsDefined(typeof(Month), firstMonth)) throw new ArgumentOutOfRangeException(nameof(firstMonth));
        }
    }
}
