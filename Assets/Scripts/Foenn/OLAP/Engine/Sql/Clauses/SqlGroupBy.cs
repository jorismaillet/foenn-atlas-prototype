namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    using Assets.Scripts.Foenn.OLAP.Engine.Sql;
    using System.Collections.Generic;

    public class SqlGroupBy
    {
        public readonly string clause;

        public SqlGroupBy(List<IDataField> groups)
        {
            var groupByParts = new List<string>();
            foreach (var column in groups)
            {
                groupByParts.Add(column.ToSql());
            }
            clause = groupByParts.Count > 0
                ? " GROUP BY " + string.Join(", ", groupByParts)
                : string.Empty;
        }
    }
}
