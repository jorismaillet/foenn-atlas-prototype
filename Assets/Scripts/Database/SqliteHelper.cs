using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;
using SqlKata;
using SqlKata.Compilers;
using UnityEngine.UIElements;

namespace Assets.Scripts.Database
{
    public class SqliteHelper
    {
        public const string DATABASE_PATH = "Resources/sqlite/foenn.db";
        public const string DATABASE_TEST_PATH = "Resources/sqlite/foenn_test.db";
        private const int SQLITE_BUSY_TIMEOUT_MS = 15000;
        private const int SQLITE_LOCK_RETRY_COUNT = 5;
        private const int SQLITE_LOCK_RETRY_DELAY_MS = 100;

        public static SqliteConnection CreateConnection()
        {
            var path = DatabaseHelper.ResolveDatabasePath(Env.DatabasePath());
            string connString = $"Data Source={path};Version=3;Pooling=True;Default Timeout=15;";
            var conn = new SqliteConnection(connString);
            conn.Open();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = $"PRAGMA busy_timeout={SQLITE_BUSY_TIMEOUT_MS};";
                command.ExecuteNonQuery();
            }
            return conn;
        }

        public static void ApplyStagingPragmas(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @$"
                PRAGMA journal_mode=WAL;
                PRAGMA synchronous=NORMAL;
                PRAGMA temp_store=MEMORY;
                PRAGMA foreign_keys=OFF;
                PRAGMA busy_timeout={SQLITE_BUSY_TIMEOUT_MS};
                PRAGMA cache_size=-200000;
            ";
            command.ExecuteNonQuery();
        }

        public static SqliteCommand GetStageCommand(SqliteConnection connection, ITable table)
        {
            var command = connection.CreateCommand();
            command.CommandText = @$"
            INSERT INTO {table.name}_staging({string.Join(", ", table.Columns.Select(column => column.name))})
            VALUES ({string.Join(", ", table.Columns.Select(column => $"@{column.name}"))})
            ";
            foreach (var column in table.Columns)
            {
                command.Parameters.Add(new SqliteParameter($"@{column.name}", column.dbType));
            }
            return command;
        }

        public static void MergeStagingTable(SqliteConnection connection, ITable table)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @$"
                INSERT OR IGNORE INTO {table.name}
                SELECT * FROM {table.name}_staging;
            ";
            ExecuteNonQueryWithRetry(command);
        }

        public static string FieldToSql(Field field, bool skipPK = false)
        {
            var res = field.dbType switch
            {
                DbType.String => "TEXT",
                DbType.Single or DbType.Double => "REAL",
                DbType.Int64 or DbType.Int32 or DbType.Int16 => "INTEGER",
                _ => throw new System.NotImplementedException($"Database type not supported: {field.dbType}")
            };
            if (!skipPK && field.isPrimaryKey)
            {
                res += " PRIMARY KEY";
                if (field.autoIncrement)
                    res += " AUTOINCREMENT";
            }
            return res;
        }

        public static bool Exists(SqliteConnection connection, string table, string column, string value)
        {
            var sql = $"SELECT COUNT(*) FROM \"{table}\" WHERE \"{column}\" = '{value}' LIMIT 1;";
            return ExecuteScalar(connection, sql) > 0;
        }

        public static void Insert(SqliteConnection connection, string table, List<Field> columns, List<string> values)
        {
            var columnsString = string.Join(", ", columns.Select(c => c.name));
            var valuesString = string.Join(", ", values.Select((val, i) => ValueToSql(val, columns[i].dbType)));
            var sql = $"INSERT INTO \"{table}\" ({columnsString}) VALUES ({valuesString});";
            Execute(connection, sql);
        }

        private static string ValueToSql(string rawValue, DbType type)
        {
            if (string.IsNullOrEmpty(rawValue)) return "NULL";
            switch (type)
            {
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

        public static void CreateTable(SqliteConnection connection, ITable table)
        {
            var columns = table.Columns.Select(column => $"\"{column.name}\" {FieldToSql(column)}");

            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.name}\" ({string.Join(", ", columns)});";

            table.Indexes.ForEach(index =>
            {
                string cols = string.Join(", ", index.fields.Select(field => $"\"{field.name}\""));
                string unique = index.unique ? "UNIQUE " : string.Empty;
                createTableSql += $"CREATE {unique}INDEX IF NOT EXISTS \"{index.name}\" ON \"{table.name}\"({cols});";
            });
            UnityEngine.Debug.Log(createTableSql);
            Execute(connection, createTableSql);
        }

        public static void CreateStagingTable(SqliteConnection connection, ITable table)
        {
            var columns = new List<string>();
            table.Columns.ForEach(column =>
            {
                columns.Add($"{column.name} {FieldToSql(column, skipPK: true)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.name}_staging\" ({string.Join(", ", columns)});";
            UnityEngine.Debug.Log(createTableSql);
            Execute(connection, createTableSql);
        }

        public static void DropStagingTable(SqliteConnection connection, ITable table)
        {
            var sql = $"DROP TABLE IF EXISTS {table.name}_staging;";
            UnityEngine.Debug.Log(sql);
            Execute(connection, sql);
        }

        public static void Execute(SqliteConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            ExecuteNonQueryWithRetry(command);
        }

        public static SqliteDataReader ExecuteReader(SqliteConnection connection, Query query)
        {
            var compiled = new SqliteCompiler().Compile(query);
            UnityEngine.Debug.Log(compiled.Sql);

            var command = connection.CreateCommand();
            command.CommandText = compiled.Sql;

            for (int i = 0; i < compiled.Bindings.Count; i++)
            {
                command.Parameters.Add(new SqliteParameter($"@p{i}", compiled.Bindings[i] ?? DBNull.Value));
            }

            return command.ExecuteReader();
        }

        public static int ExecuteScalar(SqliteConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return ExecuteScalarWithRetry(command);
        }

        private static void ExecuteNonQueryWithRetry(SqliteCommand command)
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    command.ExecuteNonQuery();
                    return;
                }
                catch (SqliteException ex) when (IsLockedException(ex) && attempt < SQLITE_LOCK_RETRY_COUNT)
                {
                    Thread.Sleep(SQLITE_LOCK_RETRY_DELAY_MS * (attempt + 1));
                }
            }
        }

        private static int ExecuteScalarWithRetry(SqliteCommand command)
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    return (int)command.ExecuteScalar();
                }
                catch (SqliteException ex) when (IsLockedException(ex) && attempt < SQLITE_LOCK_RETRY_COUNT)
                {
                    Thread.Sleep(SQLITE_LOCK_RETRY_DELAY_MS * (attempt + 1));
                }
            }
        }

        private static bool IsLockedException(SqliteException ex)
        {
            var message = ex?.Message;
            return !string.IsNullOrEmpty(message)
                && message.IndexOf("locked", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
