using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Condition
{
    public interface ICondition
    {
        public bool IsMatch(Row row);

        public QueryRequest AddToQuery(QueryRequest query);
    }
}
