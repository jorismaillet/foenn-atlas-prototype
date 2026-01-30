using Assets.Scripts.Foenn.Engine.Requests;

namespace Assets.Scripts.Foenn.Engine.SQL
{
    public class SQLGenerator
    {
        private readonly ISqlDialect dialect;
        public SQLGenerator(ISqlDialect dialect)
        {
            this.dialect = dialect;
        }

        public CompiledQuery Generate(QueryRequest request)
        {
            // TODO: Générer dynamiquement la requête SQL selon le QueryRequest et le dialecte
            // Exemple simplifié pour une agrégation SUM sur une métrique
            var emit = new SQL.SqlEmit();
            var agg = request.MetricAggregations.First();
            string aggFunc = agg.aggregationKey.ToString();
            string metric = agg.aggregatedMetrics.ToString();
            emit.Append($"SELECT {aggFunc}({dialect.QuoteIdent(metric)}) AS {metric} FROM weather_data");
            // TODO: Ajouter WHERE, GROUP BY, etc. selon les attributs et filtres
            return new SQL.CompiledQuery(emit.ToString(), emit.Parameters);
        }
    }
}