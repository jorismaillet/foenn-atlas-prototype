using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using System;

namespace Assets.Scripts.Foenn.Engine.Filters
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