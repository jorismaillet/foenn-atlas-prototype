using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;
using SqlKata;

namespace Assets.Scripts.OLAP.Datasets.Metadata
{
    public class MetadataTable : Table
    {
        private Field fileName;
		public override IEnumerable<Field> Columns { get; }

		public MetadataTable(string datasetName) : base(MakeTableName(datasetName))
        {
            fileName = Field.TextAttribute(Name, "File", "");
            Indexes.Add(new IndexDefinition(true, fileName));
            Columns = new List<Field>() { PrimaryKey, fileName };
        }

        public static string MakeTableName(string table)
        {
            return $"{table}_metadata";
        }

        public List<string> FilesToLoad(SqliteConnection connection)
        {
            var weathersDir = Path.Combine(UnityEngine.Application.dataPath, "Resources", "Weathers");
            if (!Directory.Exists(weathersDir))
            {
                throw new Exception($"Weathers folder not found: {weathersDir}");
            }

            var allFiles = Directory.EnumerateFiles(weathersDir, "*.csv", SearchOption.TopDirectoryOnly)
                .Select(f => Path.Combine("Weathers", Path.GetFileName(f)));

            var existingFiles = new List<string>();
            using (var reader = SqliteHelper.ExecuteReader(connection, new Query(Name).Select(fileName.fieldName)))
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
                this,
                new List<Field>() { fileName },
                new List<string>() { file }
            );
        }
    }
}
