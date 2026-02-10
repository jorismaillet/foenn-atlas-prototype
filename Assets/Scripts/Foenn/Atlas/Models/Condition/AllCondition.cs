using Assets.Scripts.Foenn.Engine.Execution;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
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