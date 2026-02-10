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
        public List<Attribute> attributes = new List<Attribute>();
        public List<Measure> measures = new List<Measure>();

        public Measure Measure(WeatherHistoryMetricKey key)
        {
            return measures.Find(measure => measure.metric.key.Equals(key));
        }
        public Attribute Attribute(WeatherHistoryAttributeKey key)
        {
            return attributes.Find(attribute => attribute.key.Equals(key));
        }
    }
}