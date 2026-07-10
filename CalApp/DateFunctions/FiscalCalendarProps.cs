using System;

namespace CalApp.DateFunctions
{
    public class FiscalCalendarProps
    {
        public static string FiscalYear(DateTime dateTime, DayOfWeek firstDayOfFiscalWeek,
            FiscalWeekRule fiscalWeekRule, Month firstFiscalMonth)
        {
            return FiscalData(dateTime, firstDayOfFiscalWeek, fiscalWeekRule, firstFiscalMonth)[1];
        }

        public static string FiscalWeek(DateTime dateTime, DayOfWeek firstDayOfWeek,
            FiscalWeekRule fiscalWeekRule, Month firstFiscalMonth)
        {
            return FiscalData(dateTime, firstDayOfWeek, fiscalWeekRule, firstFiscalMonth)[0];
        }

        private static string[] FiscalData(DateTime dateTime, DayOfWeek firstDayOfWeek,
            FiscalWeekRule fiscalWeekRule, Month firstFiscalMonth)
        {
            DateTime firstDayOfFirstFiscalMonthThisYear = new DateTime(dateTime.Year, (int)firstFiscalMonth, 1);
            DateTime firstDayOfFirstFiscalMonthLastYear = firstDayOfFirstFiscalMonthThisYear.AddYears(-1);
            // FIX: renamed from the misleading 'firstOct' — the first fiscal
            // month is configurable (default October), not hardcoded.
            DateTime fiscalYearStart;
            string fiscalYear;

            if ((((firstDayOfFirstFiscalMonthThisYear - GetLastWeekday(firstDayOfWeek, firstDayOfFirstFiscalMonthThisYear)).Days >= (int)fiscalWeekRule)
                && (dateTime < GetNextWeekday(firstDayOfWeek, firstDayOfFirstFiscalMonthThisYear)))
                || (((firstDayOfFirstFiscalMonthThisYear - GetLastWeekday(firstDayOfWeek, firstDayOfFirstFiscalMonthThisYear)).Days < (int)fiscalWeekRule)
                && (dateTime < GetLastWeekday(firstDayOfWeek, firstDayOfFirstFiscalMonthThisYear))))
            {
                fiscalYearStart = firstDayOfFirstFiscalMonthLastYear;
                fiscalYear = dateTime.AddYears(-1).ToString("yy") + "/" + dateTime.ToString("yy");
            }
            else
            {
                fiscalYearStart = firstDayOfFirstFiscalMonthThisYear;
                fiscalYear = dateTime.ToString("yy") + "/" + dateTime.AddYears(1).ToString("yy");
            }

            int weekNumber;
            if ((fiscalYearStart - GetLastWeekday(firstDayOfWeek, fiscalYearStart)).Days < (int)fiscalWeekRule)
                weekNumber = ((dateTime - GetLastWeekday(firstDayOfWeek, fiscalYearStart)).Days) / 7 + 1;
            else
                weekNumber = ((dateTime - GetNextWeekday(firstDayOfWeek, fiscalYearStart)).Days) / 7 + 1;

            // FIX (defensive): the branch logic above is expected to keep the
            // day-difference non-negative, so integer division gives the right
            // week. But if an edge case ever produced a date before the fiscal
            // year start, integer division truncates toward zero and could
            // yield 0 or a negative week. Clamp to a minimum of week 1 so the
            // tray never shows a nonsensical "0" or negative week number.
            if (weekNumber < 1)
                weekNumber = 1;

            return new string[] { weekNumber.ToString(), fiscalYear };
        }

        private static DateTime GetNextWeekday(DayOfWeek dayOfWeek, DateTime dateTime)
        {
            DateTime result = dateTime;
            while (result.DayOfWeek != dayOfWeek)
                result = result.AddDays(1);
            return result;
        }

        private static DateTime GetLastWeekday(DayOfWeek dayOfWeek, DateTime dateTime)
        {
            DateTime result = dateTime;
            while (result.DayOfWeek != dayOfWeek)
                result = result.AddDays(-1);
            return result;
        }
    }
}