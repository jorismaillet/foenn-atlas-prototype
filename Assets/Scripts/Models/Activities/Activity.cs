using Assets.Scripts.Models.Condition;
using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Activities
{
    public class Activity
    {
        public string name;

        public AllCondition conditions;

        public Activity(string name, params ICondition[] conditions)
        {
            this.name = name;
            this.conditions = new AllCondition(conditions);
        }

        public QueryRequest AddToQuery(QueryRequest query)
        {
            return conditions.AddToQuery(query);
        }
    }
}
