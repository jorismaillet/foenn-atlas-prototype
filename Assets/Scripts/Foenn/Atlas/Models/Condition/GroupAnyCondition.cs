namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    using Assets.Scripts.Foenn.Atlas.Models.Activities;
    using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;

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
