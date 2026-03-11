using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Sql.Filters
{
    public class ExcludeNullFilter : Filter
    {
        public ExcludeNullFilter(Field column) : base(column)
        {
        }

        public override string ToSql()
        {
            return $"{filteredField.ToSql()} IS NOT NULL";
        }
    }
}
