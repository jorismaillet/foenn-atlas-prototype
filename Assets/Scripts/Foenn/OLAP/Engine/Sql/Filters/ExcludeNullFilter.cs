namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    using Assets.Scripts.Foenn.Engine.Sql;

    public class ExcludeNullFilter : Filter
    {
        public ExcludeNullFilter(PrefixedField column) : base(column)
        {
        }

        public override string ToSql()
        {
            return $"{filteredField.ToSql()} IS NOT NULL";
        }
    }
}
