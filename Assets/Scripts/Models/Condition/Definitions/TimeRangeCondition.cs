using System;
using Assets.Scripts.Helpers;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Condition.Definitions
{
    public class TimeRangeCondition : ICondition
    {
        private TimeDimension dimension;

        public DateTime start, end;

        public TimeRangeCondition(TimeDimension dimension, DateTime min, DateTime max)
        {
            this.dimension = dimension;
            this.start = min;
            this.end = max;
        }

        public bool IsMatch(Row row)
        {
            var time = DateTimeHelper.FromTimestamp(row.StringValue(dimension.timestamp));
            var duration = row.IntValue(dimension.duration);
            var timeEnd = time.AddHours(duration);

            return time >= start && timeEnd <= end;
        }

        public QueryRequest AddToQuery(QueryRequest query)
        {
            var startTimestamp = new DateTimeOffset(start).ToUnixTimeSeconds();
            var endTimestamp = new DateTimeOffset(end).ToUnixTimeSeconds();
            return query.WhereBetween(dimension.timestamp, startTimestamp, endTimestamp);
        }
    }
}
