using System;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Condition.Definitions
{
    public class HourRangeCondition : ICondition
    {
        public int minHour, maxHour;

        public HourRangeCondition(int minHour, int maxHour)
        {
            this.minHour = minHour;
            this.maxHour = maxHour;
        }

        public bool IsMatch(Row row)
        {
            var timestamp = DateTime.Parse((string)row.values[TimeDimension.timestamp]);
            var duration = (int)row.values[TimeDimension.duration];
            return timestamp.Hour >= minHour && timestamp.AddHours(duration).Hour <= maxHour;
        }
    }
}
