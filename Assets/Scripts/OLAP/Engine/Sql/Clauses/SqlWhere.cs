using System.Collections.Generic;
using Assets.Scripts.OLAP.Engine.Sql.Filters;

namespace Assets.Scripts.OLAP.Engine.Sql.Clauses
{
    public class SqlWhere
    {
        public readonly string clause;

        public SqlWhere(List<Filter> filters)
        {
            var whereParts = new List<string>();
            foreach (var filter in filters)
            {
                whereParts.Add(filter.ToSql());
            }
            if (whereParts.Count > 0)
                clause = " WHERE " + string.Join(" AND ", whereParts);
            else
                clause = string.Empty;
        }
    }
}
