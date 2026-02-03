namespace Assets.Scripts.Foenn.Engine.Metrics.CustomMetrics
{
    public abstract class CustomMetric : AbstractMetric
    {
        // Formula -> Execution
        protected CustomMetric(AggregationKey aggregation) : base(aggregation)
        {
        }
    }
}