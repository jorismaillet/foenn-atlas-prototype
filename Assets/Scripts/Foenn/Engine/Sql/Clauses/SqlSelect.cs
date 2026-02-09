using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlSelect
    {
        public readonly string clause;
        public SqlSelect(List<Metric> metrics, ISqlDialect dialect)
        {
            var selectParts = metrics
                .Select(metric =>
                    $"{metric.aggregation}({dialect.QuoteIdent(metric.key.ToString())})"
                ).ToList();

            clause = "SELECT " + (selectParts.Any() ? string.Join(", ", selectParts) : "*");
        }
    }
}