using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.OLAP;
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

        public bool SuitsHour(Row row)
        {
            return metrics.keys.Intersect(row.Keys).Any(key =>
            {
                var value = row[key];
                return min <= value && max >= value;
            });
        }
    }
}