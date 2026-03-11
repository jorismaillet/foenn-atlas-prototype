namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Fields;

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
