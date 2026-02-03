using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public class SqlGenerator
    {
        private readonly ISqlDialect dialect;
        public SqlGenerator(ISqlDialect dialect)
        {
            this.dialect = dialect;
        }

        public CompiledQuery Generate(QueryRequest request)
        {
            var emit = new SqlEmit();

            // 1. SELECT
            var selectClause = new Clauses.SqlSelect(request, dialect);
            emit.Append(selectClause.ToString());

            // 2. FROM
            var fromClause = new Clauses.SqlFrom("weather_data", dialect);
            emit.Append(fromClause.ToString());

            // 3. WHERE
            var whereClause = new Clauses.SqlWhere(request, dialect, emit);
            var whereStr = whereClause.ToString();
            if (!string.IsNullOrWhiteSpace(whereStr))
                emit.Append(whereStr);

            // 4. GROUP BY
            var groupByClause = new Clauses.SqlGroupBy(request, dialect);
            var groupByStr = groupByClause.ToString();
            if (!string.IsNullOrWhiteSpace(groupByStr))
                emit.Append(groupByStr);

            return new CompiledQuery(emit.ToString(), emit.parameters);
        }
    }
}