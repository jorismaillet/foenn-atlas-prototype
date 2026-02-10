using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;

namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    public class Metric
    {
        public WeatherHistoryMetricKey key;
        public AggregationKey aggregation;

        public Metric(WeatherHistoryMetricKey key, AggregationKey aggregation)
        {
            this.key = key;
            this.aggregation = aggregation;
        }
    }
}