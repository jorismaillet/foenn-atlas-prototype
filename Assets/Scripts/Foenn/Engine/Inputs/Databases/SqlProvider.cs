using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Collections.Generic;
using System.Data;
using UnityEditor.PackageManager.Requests;

namespace Assets.Scripts.Foenn.Engine.Inputs.Databases
{
    public abstract class SqlProvider : IInputProvider
    {
        protected ISqlDialect dialect;
        protected IDbConnection connection;
        private string sql;
        private QueryRequest request;

        public abstract void OpenSession();
        public abstract void CloseSession();

        public SqlProvider(ISqlDialect dialect)
        {
            this.connection = null;
            this.dialect = dialect;
        }

        public void Initialize(QueryRequest request)
        {
            this.request = request;
            this.sql = request.ToSql(dialect);
        }

        public QueryResult Execute()
        {
            OpenSession();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            var columns = new List<string>();
            var rows = new List<List<string>>();
            using (var reader = command.ExecuteReader())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetName(i));
                }
                while (reader.Read())
                {
                    var row = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i)?.ToString() ?? string.Empty;
                        row.Add(value);
                    }
                    rows.Add(row);
                }
            }
            CloseSession();
            return new QueryResult(request, columns, rows);
        }
    }
}