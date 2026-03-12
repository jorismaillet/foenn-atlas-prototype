using System;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.Models.Condition.Definitions
{
    public class TimeRangeCondition : ICondition
    {
        public DateTime start, end;

        public TimeRangeCondition(DateTime min, DateTime max)
        {
            this.start = min;
            this.end = max;
        }

        public bool IsMatch(Row row)
        {
            var time = TimeDimension.FromTimestamp(row.StringValue(TimeDimension.timestamp));
            var duration = row.IntValue(TimeDimension.duration);
            var timeEnd = time.AddHours(duration);

            return time >= start && timeEnd <= end;
        }
    }
}
