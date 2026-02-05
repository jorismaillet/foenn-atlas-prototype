using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.OLAP;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public class MetricRangeCondition : ICondition
    {
        public MetricKey metricKey;
        public float min;
        public float max;

        public MetricRangeCondition(MetricKey metricKey, float min, float max)
        {
            this.metricKey = metricKey;
            this.min = min;
            this.max = max;
        }

        public bool IsMatch(Row row)
        {
            var measure = row.Measure(metricKey);
            return min <= measure.value && max >= measure.value;
        }
    }
}