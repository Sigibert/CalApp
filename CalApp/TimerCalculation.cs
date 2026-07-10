using System;

namespace CalApp
{
    public class TimerCalculation
    {
        public static int MillisecondsLeftTillNextUpdate()
        {
            DateTime now = DateTime.Now;
            DateTime nextHour = now.AddHours(1).Date.AddHours(now.AddHours(1).Hour);
            DateTime nextMidnight = now.Date.AddDays(1);

            DateTime nextTrigger = nextHour < nextMidnight ? nextHour : nextMidnight;

            int interval = (int)(nextTrigger - now).TotalMilliseconds;

            if (interval <= 0)
                interval = 60 * 60 * 1000;

            return interval;
        }
    }
}