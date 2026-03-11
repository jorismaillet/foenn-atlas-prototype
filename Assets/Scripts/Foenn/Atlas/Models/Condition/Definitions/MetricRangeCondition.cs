namespace Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions
{
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;

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
