using Assets.Scripts.Foenn.Engine.OLAP;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public class AnyCondition : ICondition
    {
        public List<ICondition> conditions;

        public AnyCondition(params ICondition[] conditions)
        {
            this.conditions = new List<ICondition>(conditions);
        }

        public bool IsMatch(Row record)
        {
            return conditions.Any(condition => condition.IsMatch(record));
        }
    }
}