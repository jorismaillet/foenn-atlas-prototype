using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlSelect
    {
        private readonly string selectClause;
        public SqlSelect(QueryRequest request, ISqlDialect dialect)
        {
            // Agrégations
            var selectParts = request.metrics
                .Select(metric =>
                    $"{metric.aggregation}({dialect.QuoteIdent(metric.aggregatedMetrics.ToString())}) AS {dialect.QuoteIdent(metric.aggregatedMetrics.ToString() + "_" + metric.aggregationKey)}"
                ).ToList();

            // Attributs groupés (ajoutés au SELECT, mais pas de GROUP BY ici)
            foreach (var attr in request.TimeAttributes.Concat(request.GeoAttributes))
            {
                selectParts.Add(dialect.QuoteIdent(attr.ToString()));
            }

            selectClause = "SELECT " + string.Join(", ", selectParts);
        }
        public override string ToString() => selectClause;
    }
}