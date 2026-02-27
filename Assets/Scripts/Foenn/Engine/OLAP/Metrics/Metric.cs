using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Drawing.Drawing2D;

namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    public class Metric
    {
        public WeatherHistoryMetricKey key;
        public AggregationKey? aggregation;

        public Metric(WeatherHistoryMetricKey key, AggregationKey? aggregation = null)
        {
            this.key = key;
            this.aggregation = aggregation;
        }
    }
}