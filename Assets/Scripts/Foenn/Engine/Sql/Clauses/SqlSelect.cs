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
            var selectParts = request.metrics
                .Select(metric =>
                    $"{metric.aggregation}({dialect.QuoteIdent(metric.aggregation.ToString())}) AS {dialect.QuoteIdent(metric.aggregation.ToString() + "_" + metric.aggregation)}"
                ).ToList();

            selectClause = "SELECT " + string.Join(", ", selectParts);
        }
        public override string ToString() => selectClause;
    }
}