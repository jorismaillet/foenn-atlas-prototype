using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor.MemoryProfiler;

namespace Assets.Scripts.Foenn.ETL.Loaders
{
    public class SqliteLoader : Loader
    {
        private string tableName;
        public SqliteConnector connector;
        private SqliteConnection connection;
        private SqliteTransaction transaction;
        private SqliteCommand command;
        private int loaded = 0, inBatch = 0, batchSize = 10000;

        public SqliteLoader(Datasource datasource, string databasePath = SqliteConnector.DATABASE_PATH) : base(datasource)
        {
            this.connector = new SqliteConnector(databasePath);
            this.tableName = datasource.TableName();
        }

        public override void StartLoad(SchemaDefinition schema)
        {
            connection = connector.OpenSession() as SqliteConnection;
            SqliteConnector.ApplyPragmas(connection);
            this.transaction = connection.BeginTransaction();
            this.command = SqliteConnector.PrepareInsert(connection, transaction, schema);
        }

        public override void LoadLine(List<string> line)
        {
            for (int i = 0; i < line.Count; i++)
            {
                command.Parameters[i].Value = SqlConnector.DbTypeToValue(command.Parameters[i].DbType, line[i]);
            }
            command.ExecuteNonQuery();
            loaded++;
            if (inBatch >= batchSize)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                inBatch = 0;
                UnityEngine.Debug.Log($"Inserted batch, total={loaded}");
            }
        }

        public override void EndLoad()
        {
            transaction.Commit();
            connection.Close();
            transaction.Dispose();
        }

        public override SqlConnector Connector()
        {
            return connector;
        }
    }
}