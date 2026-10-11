using System;

namespace WeekTrayInfo
{
    public static class RefreshSchedule
    {
        // A short heartbeat also catches resume from sleep and manual clock changes.
        public static int GetInterval(DateTime now)
        {
            double remaining = TimeSpan.FromHours(1).TotalMilliseconds - now.TimeOfDay.TotalMilliseconds % TimeSpan.FromHours(1).TotalMilliseconds;
            return Math.Max(1, (int)Math.Min(60000, Math.Ceiling(remaining)));
        }

        public static DateTime GetHour(DateTime now) => now.Date.AddHours(now.Hour);
    }
}
