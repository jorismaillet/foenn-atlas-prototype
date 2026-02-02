namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public class Metric : AbstractMetric
    {
        public MetricKey key;

        public Metric(MetricKey key, AggregationKey aggregation) : base(aggregation)
        {
            this.key = key;
        }
    }
}