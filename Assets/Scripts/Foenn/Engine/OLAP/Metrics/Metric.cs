namespace Assets.Scripts.Foenn.Engine.Metrics
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