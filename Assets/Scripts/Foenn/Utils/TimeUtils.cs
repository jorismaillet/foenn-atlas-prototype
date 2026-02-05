using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Assets.Scripts.Foenn.Utils
{
    public class TimeUtils
    {
        public static int Hour(string AAAAMMJJHH)
        {
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime.Hour;
        }

        public static string Day(string AAAAMMJJHH)
        {
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime.ToString("dd MMMM", new CultureInfo("fr-FR"));
        }

        public static int WeekOfYear(string AAAAMMJJHH)
        {
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return GetIso8601WeekOfYear(dateTime);
        }

        public static string Month(string AAAAMMJJHH)
        {
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime.ToString("MMMM", new CultureInfo("fr-FR"));
        }

        public static DateTime Date(string AAAAMMJJHH)
        {
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime;
        }

        public static string ToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHH");
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static int MaxConsecutiveHours(List<int> hours)
        {
            if (hours == null || !hours.Any())
            {
                return 0;
            }

            int maxConsecutive = 1;
            int currentStreak = 1;

            for (int i = 1; i < hours.Count; i++)
            {
                if (hours[i] == hours[i - 1] + 1)
                {
                    currentStreak++;
                }
                else
                {
                    if (currentStreak > maxConsecutive)
                    {
                        maxConsecutive = currentStreak;
                    }
                    currentStreak = 1;
                }
            }

            return Math.Max(maxConsecutive, currentStreak);
        }
    }
}