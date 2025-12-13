namespace AF
{
    using System;

    public static class SeasonalEvents
    {
        public static bool IsChristmasTime()
        {
            DateTime now = DateTime.Now;
            int month = now.Month;
            int day = now.Day;

            // December 1 → December 31
            if (month == 12)
                return true;

            // January 1 → January 6
            if (month == 1 && day <= 6)
                return true;

            return false;
        }
    }
}
