using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using UnityEngine.UIElements;

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
        public abstract void CreateStagingTable(SchemaDefinition schema);
        public abstract void DropStagingTable(SchemaDefinition schema);
        public abstract string FieldToSql(Datafield field, bool skipPK = false);

        public QueryResult ExecuteQuery(QueryRequest request)
        {
            var sql = request.ToSql(dialect);
            UnityEngine.Debug.Log($"Executing query : {sql}");
            OpenSession();
            var command = connection.CreateCommand();
            
            command.CommandText = sql;
            using (var reader = command.ExecuteReader())
            {
                int nbColumns = reader.FieldCount;
                string[] rawHeaders = new string[nbColumns];
                for (int i = 0; i < nbColumns; i++)
                {
                    rawHeaders[i] = reader.GetName(i);
                }
                var res = new QueryResult(rawHeaders);
                while (reader.Read())
                {
                    var row = new object[nbColumns];
                    reader.GetValues(row);
                    res.ParseLine(row);
                }
                CloseSession();
                return res;
            }
        }
    }
}