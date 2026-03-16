using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;
using SqlKata;
using SqlKata.Compilers;
using UnityEngine;

namespace Assets.Scripts.Database
{
    public class SqliteHelper
    {
        public const string DATABASE_PATH = "Resources/sqlite/foenn.db";
        public const string DATABASE_TEST_PATH = ":memory";

        private const int SQLITE_BUSY_TIMEOUT_MS = 15000;
        private const int SQLITE_LOCK_RETRY_COUNT = 5;
        private const int SQLITE_LOCK_RETRY_DELAY_MS = 100;

        public static string ResolveDatabasePath(string databasePath)
        {
            return Path.IsPathRooted(databasePath)
                ? databasePath
                : Path.Combine(Application.dataPath, databasePath);
        }

        public static void CreateDb()
        {
            var fullPath = ResolveDatabasePath(Env.DatabasePath());
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(fullPath))
                File.Create(fullPath).Close();
        }

        public static SqliteConnection CreateConnection()
        {
            var path = ResolveDatabasePath(Env.DatabasePath());
            string connString = Env.DatabasePath() == ":memory" ?
                "Data Source=:memory:;Version=3;New=True;Cache=Shared;" :
                $"Data Source={path};Version=3;Pooling=False;Default Timeout=15;";
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
            var mappings = table.Mappings;
            var insertFields = mappings.Select(c => c.targetField).Concat(table.References).ToList();
            var stageCommand = connection.CreateCommand();
            var colNames = string.Join(", ", insertFields.Select(c => c.fieldName));
            var paramNames = string.Join(", ", insertFields.Select(c => $"@{c.fieldName}"));
            stageCommand.CommandText = $"INSERT INTO \"{table.Name}_staging\" ({colNames}) VALUES ({paramNames})";

            var stageParams = new SqliteParameter[mappings.Count + table.References.Count];

            for (int colIndex = 0; colIndex < mappings.Count; colIndex++)
            {
                var mapping = mappings[colIndex];
                stageParams[colIndex] = new SqliteParameter($"@{mapping.targetField.fieldName}", mapping.targetField.dbType);
                stageCommand.Parameters.Add(stageParams[colIndex]);
            }
            for (int i = 0; i < table.References.Count; i++)
            {
                var refField = table.References[i];
                int colIndex = table.Mappings.Count + i;
                stageParams[colIndex] = new SqliteParameter($"@{refField.fieldName}", refField.dbType);
                stageCommand.Parameters.Add(stageParams[colIndex]);
            }
            return stageCommand;
        }

        public static void MergeStagingTable(SqliteConnection connection, ITable table)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @$"
                INSERT OR IGNORE INTO {table.Name}
                SELECT * FROM {table.Name}_staging;
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
            var columnsString = string.Join(", ", columns.Select(c => c.fieldName));
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
            var columns = table.Columns.Select(column => $"\"{column.fieldName}\" {FieldToSql(column)}");

            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.Name}\" ({string.Join(", ", columns)});";

            table.Indexes.ForEach(index =>
            {
                string cols = string.Join(", ", index.fields.Select(field => $"\"{field.fieldName}\""));
                string unique = index.unique ? "UNIQUE " : string.Empty;
                createTableSql += $"CREATE {unique}INDEX IF NOT EXISTS \"{index.name}\" ON \"{table.Name}\"({cols});";
            });
            UnityEngine.Debug.Log(createTableSql);
            Execute(connection, createTableSql);
        }

        public static void CreateStagingTable(SqliteConnection connection, ITable table)
        {
            var columns = new List<string>();
            table.Columns.ForEach(column =>
            {
                columns.Add($"{column.fieldName} {FieldToSql(column, skipPK: true)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.Name}_staging\" ({string.Join(", ", columns)});";
            Execute(connection, createTableSql);
        }

        public static void DropStagingTable(SqliteConnection connection, ITable table)
        {
            var sql = $"DROP TABLE IF EXISTS {table.Name}_staging;";
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
