using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class TimeRangeFilter : Filter
    {
        public DateTime startTime, endTime;

        public TimeRangeFilter(DateTime startTime, DateTime endTime, WeatherHistoryAttributeKey filteredAttributeKey) : base(filteredAttributeKey)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }
    }
}