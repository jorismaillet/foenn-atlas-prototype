using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class Row
    {
        public TimeDimension time;
        public GeoDimension geo;
        public Dictionary<WeatherHistoryAttributeKey, AttributeValue> attributes = new Dictionary<WeatherHistoryAttributeKey, AttributeValue>();
        public Dictionary<WeatherHistoryMetricKey, Measure> measures = new Dictionary<WeatherHistoryMetricKey, Measure>();
    }
}