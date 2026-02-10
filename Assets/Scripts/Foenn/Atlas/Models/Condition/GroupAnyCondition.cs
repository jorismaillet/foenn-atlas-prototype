using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public class GroupAnyCondition : AnyCondition
    {
        public GroupAnyCondition(MetricGroup group, float min, float max) : base()
        {
            group.keys.ForEach(key =>
            {
                conditions.Add(new MetricRangeCondition(key, min, max));
            });
        }
    }
}