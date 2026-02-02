using Assets.Scripts.Foenn.Engine.Metrics;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions
{
    public class MetricGroupCondition : IActivityCondition {
        public MetricGroup metrics;
        public float min;
        public float max;
        public ConditionImportanceKey importance;

        public MetricGroupCondition(MetricGroup metrics, float min, float max, ConditionImportanceKey importance)
        {
            this.metrics = metrics;
            this.min = min;
            this.max = max;
            this.importance = importance;
        }

        public bool Match(Dictionary<MetricKey, float> record)
        {
            return metrics.keys.Intersect(record.Keys).Any(key =>
            {
                var value = record[key];
                return min <= value && max >= value;
            });
        }
    }
}