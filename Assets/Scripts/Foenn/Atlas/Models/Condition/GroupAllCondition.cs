using Assets.Scripts.Foenn.Engine.Metrics;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public class GroupAllCondition : AllCondition
    {
        public GroupAllCondition(MetricGroup group, float min, float max) : base()
        {
            group.keys.ForEach(key =>
            {
                conditions.Add(new MetricRangeCondition(key, min, max));
            });
        }
    }
}