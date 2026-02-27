using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class TimeRangeFilter : Filter
    {
        public WeatherHistoryAttributeKey filteredAttributeKey;
        public DateTime startTime, endTime;

        public TimeRangeFilter(DateTime startTime, DateTime endTime, WeatherHistoryAttributeKey filteredAttributeKey) : base()
        {
            this.filteredAttributeKey = filteredAttributeKey;
            this.startTime = startTime;
            this.endTime = endTime;
        }
    }
}