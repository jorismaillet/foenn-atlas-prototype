using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.SQL
{
    public sealed class CompiledQuery
    {
        public CompiledQuery(string sql, IReadOnlyList<SqlParam> parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }

        public string Sql { get; }
        public IReadOnlyList<SqlParam> Parameters { get; }
    }
}