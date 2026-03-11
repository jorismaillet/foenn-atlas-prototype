using Assets.Scripts.OLAP.Engine.Result;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.Models.Condition.Definitions
{
    public class MetricRangeCondition : ICondition
    {
        public Field field;

        public float min;

        public float max;

        public MetricRangeCondition(Field field, float min, float max)
        {
            this.field = field;
            this.min = min;
            this.max = max;
        }

        public bool IsMatch(Row row)
        {
            var measure = (float)row.values[field];
            return min <= measure && max >= measure;
        }
    }
}
