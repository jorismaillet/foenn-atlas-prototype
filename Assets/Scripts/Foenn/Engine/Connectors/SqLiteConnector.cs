using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.Datasources;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Inputs.Databases
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
            var path = ResolveDatabasePath();
            CreateDb(path);
            string connString = $"Data Source={path};Version=3;";
            connection = new SqliteConnection(connString);
            connection.Open();
        }

        public override void CloseSession()
        {
            connection.Close();
        }

        private void CreateDb(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(path))
                File.Create(path).Close();
        }

        public override string typeToSql(Datatype type)
        {
            return type switch
            {
                Datatype.STRING => "CHAR(50)",
                Datatype.FLOAT => "REAL",
                Datatype.INT => "INT",
                Datatype.PRIMARY_KEY => "INTEGER PRIMARY KEY AUTOINCREMENT",
                _ => throw new System.NotImplementedException()
            };
        }

        public override bool Exists(string table, string column, string value)
        {
            var sql = $"SELECT COUNT(*) FROM \"{table}\" WHERE \"{column}\" = '{value}' LIMIT 1";
            var command = connection.CreateCommand();
            command.CommandText = sql;
            var count = (long)command.ExecuteScalar();
            return count > 0;
        }

        public override void Insert(string table, List<string> columns, List<string> values)
        {
            var columnsString = string.Join(", ", columns);
            var valuesString = string.Join(", ", values.Select(val => string.IsNullOrEmpty(val) ? "NULL" : val));
            var sql = $"INSERT INTO \"{table}\" ({columnsString}) VALUES ({valuesString})";
            ExecuteOperation(sql);
        }

        public override void CreateTable(string name, List<Datafield> fields)
        {
            var columns = new List<string>();
            fields.ForEach(field =>
            {
                columns.Add($"{field.name} {typeToSql(field.type)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{name}\" ({string.Join(", ", columns)});";
            ExecuteOperation(createTableSql);
        }
    }
}