using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Condition
{
    public class NamedCondition : ICondition
    {
        public string name;

        public ICondition condition;

        public NamedCondition(string name, ICondition condition)
        {
            this.name = name;
            this.condition = condition;
        }

        public bool IsMatch(Row row)
        {
            return condition.IsMatch(row);
        }

        public QueryRequest AddToQuery(QueryRequest query)
        {
            return condition.AddToQuery(query);
        }
    }
}
