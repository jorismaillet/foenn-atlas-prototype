using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.Models.Condition.Definitions
{
    public class FloatRangeCondition : ICondition
    {
        public Field field;

        public float min, max;

        public FloatRangeCondition(Field field, float min, float max)
        {
            this.field = field;
            this.min = min;
            this.max = max;
        }

        public bool IsMatch(Row row)
        {
            var val = row.FloatValue(field);
            return min <= val && val <= max;
        }
    }
}
