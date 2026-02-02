using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlGroupBy
    {
        private readonly string groupByClause;

        public SqlGroupBy(QueryRequest request, ISqlDialect dialect)
        {
            var groupByParts = new List<string>();
            foreach (var attr in request.TimeAttributes.Concat(request.GeoAttributes))
            {
                groupByParts.Add(dialect.QuoteIdent(attr.ToString()));
            }
            groupByClause = groupByParts.Count > 0
                ? " GROUP BY " + string.Join(", ", groupByParts)
                : string.Empty;
        }
        public override string ToString() => groupByClause;
    }
}