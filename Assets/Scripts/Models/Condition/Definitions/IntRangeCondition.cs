using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.Models.Condition.Definitions
{
    public class IntRangeCondition : ICondition
    {
        public Field field;

        public int min, max;

        public IntRangeCondition(Field field, int min, int max)
        {
            this.field = field;
            this.min = min;
            this.max = max;
        }

        public bool IsMatch(Row row)
        {
            var val = row.IntValue(field);
            return min <= val && val <= max;
        }

        public QueryRequest AddToQuery(QueryRequest query)
        {
            return query.WhereBetween(field, min, max);
        }
    }
}
