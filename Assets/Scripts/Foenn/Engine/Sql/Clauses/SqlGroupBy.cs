using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlGroupBy
    {
        public readonly string clause;
        public SqlGroupBy(List<WeatherHistoryAttributeKey> groups, ISqlDialect dialect)
        {
            var groupByParts = new List<string>();
            foreach (var attr in groups)
            {
                groupByParts.Add(dialect.QuoteIdent(attr.ToString()));
            }
            clause = groupByParts.Count > 0
                ? " GROUP BY " + string.Join(", ", groupByParts)
                : string.Empty;
        }
    }
}