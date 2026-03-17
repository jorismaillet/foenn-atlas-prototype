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

        public static string ResolveDatabasePath(string databasePath)
        {
            return Path.IsPathRooted(databasePath)
                ? databasePath
                : Path.Combine(Application.dataPath, databasePath);
        }

        public static void CreateDb()
        {
            var fullPath = ResolveDatabasePath(Env.DatabasePath);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(fullPath))
                File.Create(fullPath).Close();
        }

        public static SqliteConnection CreateConnection()
        {
            var path = ResolveDatabasePath(Env.DatabasePath);
            string connString = Env.DatabasePath == ":memory" ?
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

        public static SqliteCommand GetStageCommand(SqliteConnection connection, Table table)
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

        public static void MergeStagingTable(SqliteConnection connection, Table table)
        {
            var sql = @$"
                INSERT OR IGNORE INTO {table.Name}
                SELECT * FROM {table.Name}_staging;
            ";
            ExecuteRaw(connection, sql);
        }

        public static void Insert(SqliteConnection connection, Table table, List<Field> columns, List<string> values)
        {
            var data = new Dictionary<string, object>(columns.Count);
            for (int i = 0; i < columns.Count; i++)
                data[columns[i].fieldName] = string.IsNullOrEmpty(values[i]) ? null : (object)values[i];
            ExecuteNonQuery(connection, new Query(table.Name).AsInsert(data));
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

        public static bool Exists(SqliteConnection connection, Table table, Field field, string value)
        {
            var query = new Query(table.Name)
                .Where(field.fieldName, value)
                .Limit(1);
            return ExecuteScalar(connection, query) > 0;
        }

        private static void ExecuteNonQuery(SqliteConnection connection, Query query)
        {
            using var command = CreateCommand(connection, query);
            command.ExecuteNonQuery();
        }

        public static void CreateTable(SqliteConnection connection, Table table)
        {
            var columns = table.Columns.Select(column => $"\"{column.fieldName}\" {FieldToSql(column)}");

            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.Name}\" ({string.Join(", ", columns)});";

            table.Indexes.ForEach(index =>
            {
                string cols = string.Join(", ", index.fields.Select(field => $"\"{field.fieldName}\""));
                string unique = index.unique ? "UNIQUE " : string.Empty;
                createTableSql += $"CREATE {unique}INDEX IF NOT EXISTS \"{index.name}\" ON \"{table.Name}\"({cols});";
            });
            Debug.Log(createTableSql);
            ExecuteRaw(connection, createTableSql);
        }

        public static void CreateStagingTable(SqliteConnection connection, Table table)
        {
            var columns = new List<string>();
            foreach (var column in table.Columns)
            {
                columns.Add($"{column.fieldName} {FieldToSql(column, skipPK: true)}");
            }
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{table.Name}_staging\" ({string.Join(", ", columns)});";
            ExecuteRaw(connection, createTableSql);
        }

        public static void DropStagingTable(SqliteConnection connection, Table table)
        {
            var sql = $"DROP TABLE IF EXISTS {table.Name}_staging;";
            ExecuteRaw(connection, sql);
        }

        private static void ExecuteRaw(SqliteConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        public static SqliteDataReader ExecuteReader(SqliteConnection connection, Query query)
        {
            var command = CreateCommand(connection, query);
            return command.ExecuteReader();
        }

        public static int ExecuteScalar(SqliteConnection connection, Query query)
        {
            using var command = CreateCommand(connection, query);
            return (int)command.ExecuteScalar();
        }

        private static SqliteCommand CreateCommand(SqliteConnection connection, Query query)
        {
            var compiled = new SqliteCompiler().Compile(query);
            UnityEngine.Debug.Log(compiled.Sql);

            var command = connection.CreateCommand();
            command.CommandText = compiled.Sql;

            for (int i = 0; i < compiled.Bindings.Count; i++)
            {
                command.Parameters.Add(new SqliteParameter($"@p{i}", compiled.Bindings[i] ?? DBNull.Value));
            }

            return command;
        }
    }
}
