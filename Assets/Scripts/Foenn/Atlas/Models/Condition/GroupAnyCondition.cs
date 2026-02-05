using Assets.Scripts.Foenn.Engine.Metrics;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public class GroupAnyCondition : AnyCondition
    {
        public GroupAnyCondition(MetricGroup group, float min, float max)
        {
            group.keys.ForEach(key =>
            {
                conditions.Add(new MetricRangeCondition(key, min, max));
            });
        }
    }
}