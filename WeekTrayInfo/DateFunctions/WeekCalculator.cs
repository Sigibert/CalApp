using System;

namespace WeekTrayInfo.DateFunctions
{
    public sealed class WeekInfo
    {
        public DateTime Date { get; }
        public int CalendarWeek { get; }
        public FiscalDate Fiscal { get; }

        internal WeekInfo(DateTime date, int calendarWeek, FiscalDate fiscal)
        {
            Date = date.Date;
            CalendarWeek = calendarWeek;
            Fiscal = fiscal;
        }
    }

    public static class WeekCalculator
    {
        public static WeekInfo GetWeeks(DateTime date, DayOfWeek firstFiscalWeekday,
            FiscalWeekRule rule, Month firstFiscalMonth)
        {
            return new WeekInfo(date, IsoCalendar.GetWeek(date),
                FiscalCalendar.GetDate(date, firstFiscalWeekday, rule, firstFiscalMonth));
        }
    }
}
