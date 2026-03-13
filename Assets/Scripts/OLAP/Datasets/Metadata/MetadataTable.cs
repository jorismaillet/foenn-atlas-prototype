using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;
using SqlKata;

namespace Assets.Scripts.OLAP.Datasets.Metadata
{
    public class MetadataTable : ITable
    {
        public string name { get; set; }

        public Field PrimaryKey => Field.PK(name);

        private Field fileName;
        public List<Field> References => new List<Field>();

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, fileName),
        };

        public List<Field> Columns => new List<Field>() { PrimaryKey, fileName };
        
        public List<FieldMap> Mappings => new List<FieldMap>();

        public MetadataTable(string table)
        {
            name = MakeTableName(table);
            fileName = Field.TextAttribute(name, "File");
        }

        public static string MakeTableName(string table)
        {
            return $"{table}_metadata";
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
            using (var reader = SqliteHelper.ExecuteReader(connection, new Query(name).Select(fileName.name)))
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
                new List<string>() { file }
            );
        }
    }
}
