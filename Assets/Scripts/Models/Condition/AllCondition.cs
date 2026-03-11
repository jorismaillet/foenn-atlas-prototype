namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    using Assets.Scripts.Foenn.OLAP.Query;
    using System.Collections.Generic;
    using System.Linq;

    public class AllCondition : ICondition
    {
        public List<ICondition> conditions;

        public AllCondition(params ICondition[] conditions)
        {
            this.conditions = new List<ICondition>(conditions);
        }

        public bool IsMatch(Row record)
        {
            return conditions.All(condition => condition.IsMatch(record));
        }
    }
}
