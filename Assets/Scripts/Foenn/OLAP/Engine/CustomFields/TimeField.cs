namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System;

    public class TimeField
    {
        public DateTime start;
        public int durationHours;

        public static TimeField AAAAMMJJHH(string AAAAMMJJHH)
        {
            return new TimeField()
            {
                start = DateTime.ParseExact(AAAAMMJJHH, "yyyyMMddHH", null),
                durationHours = 1
            };
        }

        public DateTime End() => start.AddHours(durationHours);
    }
}
