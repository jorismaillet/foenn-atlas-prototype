namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public abstract class AbstractMetric
    {
        public AggregationKey aggregation;

        protected AbstractMetric(AggregationKey aggregation)
        {
            this.aggregation = aggregation;
        }
    }
}