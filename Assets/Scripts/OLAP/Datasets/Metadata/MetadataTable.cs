using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

namespace Assets.Scripts.OLAP.Datasets.Metadata
{
    public class MetadataTable : ITable
    {
        public string name { get; set; }

        public Field PrimaryKey => Field.PK();

        private Field fileName = Field.Text("File");

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, fileName),
        };

        public List<FieldMap> Mappings => new List<FieldMap>();

        public List<Field> References => new List<Field>();

        public MetadataTable(string table)
        {
            name = MakeTableName(table);
        }

        public static string MakeTableName(string table)
        {
            return $"{table}_metadata";
        }

        public void InitTable(SqliteConnection connection)
        {
            SqliteHelper.CreateTable(connection, this);
        }

        public List<string> FilesToLoad(SqliteConnection connection)
        {
            var connector = new SqliteHelper();

            var weathersDir = Path.Combine(UnityEngine.Application.dataPath, "Resources", "Weathers");
            if (!Directory.Exists(weathersDir))
            {
                throw new Exception($"Weathers folder not found: {weathersDir}");
            }

            var allFiles = Directory.EnumerateFiles(weathersDir, "*.csv", SearchOption.TopDirectoryOnly)
                .Select(f => Path.Combine("Weathers", Path.GetFileName(f)));

            var existingFiles = new List<string>();
            using (var reader = SqliteHelper.ExecuteReader(connection, $"SELECT \"{fileName.name}\" FROM {name}"))
            {
                while (reader.Read())
                {
                    existingFiles.Add((string)reader.GetValue(0));
                }
            }
            return allFiles.Except(existingFiles).ToList();
        }

        public void FlagProcessed(SqliteConnection connection, string file)
        {
            SqliteHelper.Insert(
                connection,
                name,
                new List<Field>() { fileName },
                new List<string>() { "", file }
            );
        }
    }
}
