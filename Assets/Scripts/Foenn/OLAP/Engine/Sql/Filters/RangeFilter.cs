namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    using Assets.Scripts.Foenn.Engine.Sql;

    public class RangeFilter : Filter
    {
        public int minValue, maxValue;

        public RangeFilter(PrefixedField column, int minValue, int maxValue) : base(column)
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
