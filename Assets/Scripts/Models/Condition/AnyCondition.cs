using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Condition
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
