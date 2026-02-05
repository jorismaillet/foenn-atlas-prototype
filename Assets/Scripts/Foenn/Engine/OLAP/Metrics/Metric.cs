namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public class Metric
    {
        public MetricKey key;
        public AggregationKey aggregation;

        public Metric(MetricKey key, AggregationKey aggregation)
        {
            this.key = key;
            this.aggregation = aggregation;
        }
    }
}