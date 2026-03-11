namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Mono.Data.Sqlite;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class MetadataTable : ITable
    {
        private string metadataTableName;
        private Field fileName = Field.Text("File");

        public Field PrimaryKey => Field.PK();

        public MetadataTable(string table)
        {
            metadataTableName = MakeTableName(table);
        }

        public void InitTable(SqliteConnection connection)
        {
            SqliteHelper.CreateTable(connection, this);
        }

        public static string MakeTableName(string table) => $"{table}_metadata";

        public string TableName => metadataTableName;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, fileName),
        };

        public List<Field> Columns => new List<Field>() { PrimaryKey, fileName };

                public List<FieldMapping> Mappings => new List<FieldMapping>();

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
            using (var reader = SqliteHelper.ExecuteReader(connection, $"SELECT \"{fileName.name}\" FROM {metadataTableName}"))
            {
                while (reader.Read())
                {
                    existingFiles.Add((string)reader.GetValue(0));
                }
            }
            return allFiles.Except(existingFiles).ToList();
        }

        public void FlagProcessed(SqliteConnection connection, string fileName)
        {
            SqliteHelper.Insert(
                connection,
                metadataTableName,
                Columns,
                new List<string>() { "", fileName }
            );
        }
    }
}
