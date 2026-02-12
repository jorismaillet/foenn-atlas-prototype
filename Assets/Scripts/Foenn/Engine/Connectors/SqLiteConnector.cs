using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

namespace Assets.Scripts.Foenn.Engine.Connectors
{
    public class SqliteConnector : SqlConnector
    {
        private readonly string databasePath;

        public const string DATABASE_PATH = "Resources/sqlite/foenn.db";
        public const string DATABASE_TEST_PATH = "Resources/sqlite/foenn_test.db";

        public SqliteConnector() : base(new SqliteDialect())
        {
            this.databasePath = Env.DatabasePath();
        }

        private string ResolveDatabasePath()
        {
            return Path.IsPathRooted(databasePath)
                ? databasePath
                : Path.Combine(Application.dataPath, databasePath);
        }

        public override void OpenSession()
        {
            CloseSession();
            var path = ResolveDatabasePath();
            CreateDb(path);
            string connString = $"Data Source={path};Version=3;";
            connection = new SqliteConnection(connString);
            connection.Open();
        }

        public static void ApplyStagingPragmas()
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                PRAGMA journal_mode=OFF;
                PRAGMA synchronous=OFF;
                PRAGMA temp_store=MEMORY;
                PRAGMA foreign_keys=OFF;
                PRAGMA busy_timeout=2000;
                PRAGMA temp_store=MEMORY;
                PRAGMA cache_size=-200000;
                PRAGMA locking_mode=EXCLUSIVE; 
            ";
            command.ExecuteNonQuery();
        }

        public static SqliteCommand PrepareStaging(SqliteTransaction transaction, SchemaDefinition schema)
        {
            var command = (connection as SqliteConnection).CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @$"
            INSERT INTO {schema.tableName}_staging({string.Join(", ", schema.columns.Select(h => h.name))})
            VALUES ({string.Join(", ", schema.columns.Select(h => $"@{h.name}"))})
            ";
            schema.columns.Each(header =>
            {
                command.Parameters.Add(new SqliteParameter($"@{header.name}", header.type));
            });
            return command;
        }

        public void MergeStaging(SchemaDefinition schema)
        {
            var command = connection.CreateCommand();
            command.CommandText = @$"
                INSERT OR IGNORE INTO {schema.tableName}
                SELECT * FROM {schema.tableName}_staging;
            ";
            command.ExecuteNonQuery();
        }

        public override void CloseSession()
        {
            if(connection == null) return;
            connection.Close();
            connection.Dispose();
            connection = null;
        }

        private void CreateDb(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(path))
                File.Create(path).Close();
        }

        public override string FieldToSql(Datafield field, bool skipPK = false)
        {
            var res = field.type switch
            {
                DbType.String => "TEXT",
                DbType.Single => "REAL",
                DbType.Int64 => "INTEGER",
                _ => throw new System.NotImplementedException()
            };
            if (!skipPK && field is PrimaryKey pk)
            {
                res += " PRIMARY KEY";
                if (pk.autoIncrement)
                    res += " AUTOINCREMENT";
            }
            return res;
        }

        public override bool Exists(string table, string column, string value)
        {
            var sql = $"SELECT COUNT(*) FROM \"{table}\" WHERE \"{column}\" = '{value}' LIMIT 1;";
            OpenSession();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            var count = (long)command.ExecuteScalar();
            CloseSession();
            return count > 0;
        }

        public override void Insert(string table, List<Datafield> columns, List<string> values)
        {
            var columnsString = string.Join(", ", columns.Select(c => c.name));
            var valuesString = string.Join(", ", values.Select((val, i) => ValueToSql(val, columns[i].type)));
            var sql = $"INSERT INTO \"{table}\" ({columnsString}) VALUES ({valuesString});";
            ExecuteOperation(sql);
        }

        private string ValueToSql(string rawValue, DbType type)
        {
            if (string.IsNullOrEmpty(rawValue)) return "NULL";
            switch (type)
            {
                case DbType.String:
                    return $"\"{rawValue}\"";
                case DbType.Single:
                case DbType.Int64:
                    return rawValue;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public override void CreateTable(SchemaDefinition schema)
        {
            var columns = new List<string>();
            schema.columns.ForEach(field =>
            {
                columns.Add($"{field.name} {FieldToSql(field)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{schema.tableName}\" ({string.Join(", ", columns)});";
            schema.indexes.ForEach(index =>
            {
                createTableSql += $"CREATE UNIQUE INDEX IF NOT EXISTS index_{index.name} ON {schema.tableName}({index.name});";
            });
            UnityEngine.Debug.Log(createTableSql);
            ExecuteOperation(createTableSql);
        }
        public override void CreateStagingTable(SchemaDefinition schema)
        {
            var columns = new List<string>();
            schema.columns.ForEach(field =>
            {
                columns.Add($"{field.name} {FieldToSql(field, skipPK: true)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{schema.tableName}_staging\" ({string.Join(", ", columns)});";
            UnityEngine.Debug.Log(createTableSql);
            ExecuteOperation(createTableSql);
        }
        public override void DropStagingTable(SchemaDefinition schema)
        {
            var sql = $"DROP TABLE IF EXISTS {schema.tableName}_staging;";
            UnityEngine.Debug.Log(sql);
            ExecuteOperation(sql);
        }
    }
}