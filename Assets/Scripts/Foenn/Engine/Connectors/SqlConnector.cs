using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Assets.Scripts.Foenn.Engine.Connectors
{
    public abstract class SqlConnector
    {
        public ISqlDialect dialect;
        public static IDbConnection connection;

        public abstract void OpenSession();
        public abstract void CloseSession();

        public SqlConnector(ISqlDialect dialect)
        {
            this.dialect = dialect;
        }

        public void ExecuteOperation(string sql)
        {
            OpenSession();
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            CloseSession();
        }

        public abstract bool Exists(string table, string column, string value);
        public abstract void Insert(string table, List<Datafield> columns, List<string> values);
        public abstract void CreateTable(SchemaDefinition schema);
        public abstract string typeToSql(Datatype type);

        public static object DbTypeToValue(DbType type, string rawValue)
        {
            return type switch
            {
                DbType.String => rawValue,
                DbType.Single => string.IsNullOrEmpty(rawValue) ? null : float.Parse(rawValue, CultureInfo.InvariantCulture),
                DbType.Int32 => string.IsNullOrEmpty(rawValue) ? null : int.Parse(rawValue),
                _ => throw new System.NotImplementedException()
            };
        }

        public QueryResult ExecuteQuery(QueryRequest request)
        {
            var sql = request.ToSql(dialect);
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