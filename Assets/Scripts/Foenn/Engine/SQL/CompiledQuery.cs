using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public class CompiledQuery
    {
        public string sql;
        public List<SqlParam> parameters;

        public CompiledQuery(string sql, List<SqlParam> parameters)
        {
            this.sql = sql;
            this.parameters = parameters;
        }
    }
}