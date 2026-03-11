namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Schema;

    public class ExcludeNullFilter : Filter
    {
        public ExcludeNullFilter(Field column) : base(column) { }

        public override string ToSql()
        {
            return $"{filteredField.ToSql()} IS NOT NULL";
        }
    }
}
