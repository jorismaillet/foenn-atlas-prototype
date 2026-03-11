using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Condition.Definitions;

namespace Assets.Scripts.Models.Condition
{
    public class GroupAnyCondition : AnyCondition
    {
        public GroupAnyCondition(MetricGroup group, float min, float max) : base()
        {
            group.metrics.ForEach(key =>
            {
                conditions.Add(new MetricRangeCondition(key, min, max));
            });
        }
    }
}
