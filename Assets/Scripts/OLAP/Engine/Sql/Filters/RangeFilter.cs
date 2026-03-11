using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Sql.Filters
{
    public class RangeFilter : Filter
    {
        public int minValue, maxValue;

        public RangeFilter(Field column, int minValue, int maxValue) : base(column)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override string ToSql()
        {
            return $"{filteredField.ToSql()} >= {minValue} AND {filteredField.ToSql()} <= {maxValue}";
        }
    }
}
