using Assets.Scripts.Foenn.Engine.Attributes;

namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public abstract class AbstractMetric
    {
        public MetricKey key;
        public AggregationKey aggregation;
        public abstract float Value();
    }
}