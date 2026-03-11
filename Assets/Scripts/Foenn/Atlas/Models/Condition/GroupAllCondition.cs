namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
    using Assets.Scripts.Foenn.OLAP.Query;

    public class GroupAllCondition : AllCondition
    {
        public GroupAllCondition(MetricGroup group, float min, float max) : base()
        {
            group.metrics.ForEach(key =>
            {
                conditions.Add(new MetricRangeCondition(key, min, max));
            });
        }
    }
}
