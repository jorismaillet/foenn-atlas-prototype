using Assets.Scripts.Foenn.Engine.Attributes;
using System;

namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Times
{
    public class TimeDimension
    {
        public DateTime start;
        public int durationHours;

        public static TimeDimension AAAAMMJJHH(string AAAAMMJJHH)
        {
            return new TimeDimension() {
                start = DateTime.ParseExact(AAAAMMJJHH, "yyyyMMddHH", null),
                durationHours = 1
            };
        }

        public DateTime End() {
            return start.AddHours(durationHours);
        }
    }
}