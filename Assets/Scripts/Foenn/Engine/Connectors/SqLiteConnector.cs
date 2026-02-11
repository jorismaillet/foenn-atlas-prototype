using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.Models;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Foenn.Engine.Connectors
{
    public class SqliteConnector : SqlConnector
    {

        public string databasePath;

        public const string DATABASE_PATH = "Resources/sqlite/foenn.db";

        public SqliteConnector(string databasePath = DATABASE_PATH) : base(new SqliteDialect())
        {
            this.databasePath = databasePath;
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

        public static void ApplyPragmas(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                PRAGMA journal_mode=WAL;
                PRAGMA synchronous=NORMAL;
                PRAGMA temp_store=MEMORY;
                PRAGMA foreign_keys=ON;
                PRAGMA busy_timeout=2000;
                PRAGMA temp_store=MEMORY;
                PRAGMA cache_size=-200000;
                PRAGMA locking_mode=EXCLUSIVE; 
            ";
            command.ExecuteNonQuery();
        }

        public static SqliteCommand PrepareInsert(SqliteConnection connexion, SqliteTransaction transaction, SchemaDefinition schema)
        {
            var command = connexion.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @$"
            INSERT OR IGNORE INTO {schema.tableName}({string.Join(", ", schema.headers.Select(h => h.name))})
            VALUES ({string.Join(", ", schema.headers.Select(h => $"@{h.name}"))})
            ";
            schema.headers.Each(header =>
            {
                command.Parameters.Add(new SqliteParameter($"@{header.name}", TypeToDbParam(header.type)));
            });
            return command;
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

        public static DbType TypeToDbParam(Datatype type)
        {
            return type switch
            {
                Datatype.STRING => DbType.String,
                Datatype.FLOAT => DbType.Single,
                Datatype.INT => DbType.Int32,
                Datatype.PRIMARY_KEY => DbType.Int32,
                _ => throw new System.NotImplementedException()
            };
        }

        public override string typeToSql(Datatype type)
        {
            return type switch
            {
                Datatype.STRING => "TEXT",
                Datatype.FLOAT => "REAL",
                Datatype.INT => "INT",
                Datatype.PRIMARY_KEY => "INTEGER PRIMARY KEY AUTOINCREMENT",
                _ => throw new System.NotImplementedException()
            };
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
            var valuesString = string.Join(", ", values.Select((val, i) => ValToString(val, columns[i].type)));
            var sql = $"INSERT INTO \"{table}\" ({columnsString}) VALUES ({valuesString});";
            ExecuteOperation(sql);
        }

        private string ValToString(string rawValue, Datatype type)
        {
            if (string.IsNullOrEmpty(rawValue)) return "NULL";
            switch (type)
            {
                case Datatype.STRING:
                    return $"\"{rawValue}\"";
                case Datatype.FLOAT:
                case Datatype.INT:
                case Datatype.PRIMARY_KEY:
                    return rawValue;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public override void CreateTable(SchemaDefinition schema)
        {
            var columns = new List<string>();
            schema.headers.ForEach(field =>
            {
                columns.Add($"{field.name} {typeToSql(field.type)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{schema.tableName}\" ({string.Join(", ", columns)});";
            schema.indexes.ForEach(index =>
            {
                createTableSql += $"CREATE UNIQUE INDEX IF NOT EXISTS {index.name} ON {schema.tableName}({index.column});";
            });
            ExecuteOperation(createTableSql);
        }
    }
}