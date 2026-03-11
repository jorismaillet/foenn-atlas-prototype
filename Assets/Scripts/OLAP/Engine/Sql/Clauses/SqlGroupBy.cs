using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Sql.Clauses
{
    public class SqlGroupBy
    {
        public readonly string clause;

        public SqlGroupBy(List<Field> groups)
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
