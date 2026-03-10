using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
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