using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine.UIElements;

namespace Assets.Scripts.Foenn.ETL.Loaders
{
    public class SqliteLoader : Loader
    {
        private string tableName;
        public SqliteConnector connector;
        private SqliteTransaction transaction;
        private SqliteCommand command;
        private int loaded = 0, inBatch = 0, batchSize = 10000;

        public SqliteLoader(Datasource datasource) : base(datasource)
        {
            this.connector = new SqliteConnector();
            this.tableName = datasource.TableName();
        }

        public override void StartStaging(SchemaDefinition schema)
        {
            connector.OpenSession();
            var sqliteConnection = SqlConnector.connection as SqliteConnection;
            SqliteConnector.ApplyStagingPragmas();
            this.transaction = sqliteConnection.BeginTransaction();
            this.command = SqliteConnector.PrepareStaging(transaction, schema);
            var n = command.Parameters.Count;
            _p = new SqliteParameter[n];
            _types = new DbType[n];
            _conv = new Func<string, object>[n];
            for (int i = 0; i < _p.Length; i++)
            {
                _p[i] = command.Parameters[i];
                _types[i] = _p[i].DbType;
                _conv[i] = _types[i] switch
                {
                    DbType.String => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : s,
                    DbType.Single => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : float.Parse(s, CultureInfo.InvariantCulture),
                    DbType.Int64 => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : int.Parse(s, CultureInfo.InvariantCulture),
                    _ => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : s
                };
            }

        }

        private SqliteParameter[] _p;
        private DbType[] _types;
        private Func<string, object>[] _conv;

        public override void StageLine(string[] line, string[] extraLines)
        {
            var column = 0;
            for (int i = 0; i < line.Length; i++)
            {
                _p[i].Value = _conv[i](line[i]);
                column++;
            }
            for (int i = 0; i < extraLines.Length; i++)
            {
                _p[column].Value = _conv[column](extraLines[i]);
                column++;
            }
            command.ExecuteNonQuery();
            loaded++;
            inBatch++;
            if (inBatch >= batchSize)
            {
                CommitStaging();
                transaction = (SqlConnector.connection as SqliteConnection).BeginTransaction();
                command.Transaction = transaction;
                inBatch = 0;
                MainThreadLog.Log($"Inserted batch, total={loaded}");
            }
        }

        public override void EndStaging()
        {
            try
            {
                CommitStaging();
            }
            finally
            {
                command.Dispose();
                connector.CloseSession();
                loaded = 0;
            }
        }

        public override void CommitStaging()
        {
            try
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
            finally
            {
                inBatch = 0;
            }
        }

        public override void MergeStaging(SchemaDefinition schema)
        {
            connector.OpenSession();
            var sqliteConnection = SqlConnector.connection as SqliteConnection;
            this.transaction = sqliteConnection.BeginTransaction();
            connector.MergeStaging(schema);
            transaction.Commit();
            transaction.Dispose();
            connector.CloseSession();
        }

        public override SqlConnector Connector()
        {
            return connector;
        }
    }
}