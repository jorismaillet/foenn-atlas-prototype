using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Models;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Datasources
{
    public class MetadataTable : ITable
    {
        private string metadataTableName;

        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        private Field fileName = new Field("File", DbType.String, ColumnType.ATTRIBUTE);

        public MetadataTable(string table) {
            metadataTableName = TableName(table);
        }

        public void InitTable(SqliteConnection connection)
        {
            SqliteHelper.CreateTable(connection, this);
        }

        public static string TableName(string table) => $"{table}_metadata";

        public string Name => metadataTableName;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, fileName),
        };

        public List<Field> Columns => new List<Field>() { PrimaryKey, fileName };

        public List<Reference> References => new List<Reference>();

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
