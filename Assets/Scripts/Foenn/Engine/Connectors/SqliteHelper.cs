using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.ETL.Models;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Connectors
{
    public class SqliteHelper
    {
        public const string DATABASE_PATH = "Resources/sqlite/foenn.db";
        public const string DATABASE_TEST_PATH = "Resources/sqlite/foenn_test.db";

        public static SqliteConnection CreateConnection() {
            var path = DatabaseHelper.ResolveDatabasePath(Env.DatabasePath());
            string connString = $"Data Source={path};Version=3;";
            return new SqliteConnection(connString);
        }

        public static void ApplyStagingPragmas(SqliteConnection connection) {
            var command = connection.CreateCommand();
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

        public static SqliteCommand GetStageCommand(SqliteConnection connection, ITable table) {
            var command = connection.CreateCommand();
            command.CommandText = @$"
            INSERT INTO {table.Name}_staging({string.Join(", ", table.Columns.Select(column => column.name))})
            VALUES ({string.Join(", ", table.Columns.Select(column => $"@{column.name}"))})
            ";
            table.Columns.Each(column => {
                command.Parameters.Add(new SqliteParameter($"@{column.name}", column.dbType));
            });
            return command;
        }

        public static void MergeStagingTable(SqliteConnection connection, ITable table) {
            var command = connection.CreateCommand();
            command.CommandText = @$"
                INSERT OR IGNORE INTO {table.Name}
                SELECT * FROM {table.Name}_staging;
            ";
            command.ExecuteNonQuery();
        }

        public static string FieldToSql(Datafield field, bool skipPK = false) {
            var res = field.dbType switch {
                DbType.String => "TEXT",
                DbType.Single or DbType.Double => "REAL",
                DbType.Int64 or DbType.Int32 or DbType.Int16 => "INTEGER",
                _ => throw new System.NotImplementedException($"Database type not supported: {field.dbType}")
            };
            if (!skipPK && field is PrimaryKey pk) {
                res += " PRIMARY KEY";
                if (pk.autoIncrement)
                    res += " AUTOINCREMENT";
            }
            return res;
        }

        public static bool Exists(SqliteConnection connection, string table, string column, string value) {
            var sql = $"SELECT COUNT(*) FROM \"{table}\" WHERE \"{column}\" = '{value}' LIMIT 1;";
            return ExecuteScalar(connection, sql) > 0;
        }

        public static void Insert(SqliteConnection connection, string table, List<Datafield> columns, List<string> values) {
            var columnsString = string.Join(", ", columns.Select(c => c.name));
            var valuesString = string.Join(", ", values.Select((val, i) => ValueToSql(val, columns[i].dbType)));
            var sql = $"INSERT INTO \"{table}\" ({columnsString}) VALUES ({valuesString});";
            Execute(connection, sql);
        }

        private static string ValueToSql(string rawValue, DbType type) {
            if (string.IsNullOrEmpty(rawValue)) return "NULL";
            switch (type) {
                case DbType.String:
                    return $"\"{rawValue}\"";
                case DbType.Single:
                case DbType.Double:
                case DbType.Int64:
                case DbType.Int32:
                case DbType.Int16:
                    return rawValue;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public static void CreateTable(SqliteConnection connection, ITable table) {
            var columns = table.Columns.Select(column => $"\"{column.name}\" {FieldToSql(column)}");

            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.Name}\" ({string.Join(", ", columns)});";

            table.Indexes.ForEach(index => {
                string cols = string.Join(", ", index.fields.Select(field => $"\"{field.name}\""));
                string unique = index.unique ? "UNIQUE " : string.Empty;
                createTableSql += $"CREATE {unique}INDEX IF NOT EXISTS \"{index.name}\" ON \"{table.Name}\"({cols});";
            });
            UnityEngine.Debug.Log(createTableSql);
            Execute(connection, createTableSql);
        }

        public static void CreateStagingTable(SqliteConnection connection, ITable table) {
            var columns = new List<string>();
            table.Columns.ForEach(column => {
                columns.Add($"{column.name} {FieldToSql(column, skipPK: true)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.Name}_staging\" ({string.Join(", ", columns)});";
            UnityEngine.Debug.Log(createTableSql);
            Execute(connection, createTableSql);
        }

        public static void DropStagingTable(SqliteConnection connection, ITable table) {
            var sql = $"DROP TABLE IF EXISTS {table.Name}_staging;";
            UnityEngine.Debug.Log(sql);
            Execute(connection, sql);
        }

        public static void Execute(SqliteConnection connection, string sql) {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        public static SqliteDataReader ExecuteReader(SqliteConnection connection, string sql) {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            return command.ExecuteReader();
        }

        public static int ExecuteScalar(SqliteConnection connection, string sql) {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            return (int)command.ExecuteScalar();
        }
    }
}