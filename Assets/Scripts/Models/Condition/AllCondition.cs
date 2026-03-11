using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Engine.Result;

namespace Assets.Scripts.Models.Condition
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
