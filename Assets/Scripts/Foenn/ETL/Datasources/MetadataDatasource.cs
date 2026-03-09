using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.ETL.Datasources
{
    public class MetadataDatasource
    {
        private PrimaryKey metadataID = new PrimaryKey("ID", DbType.Int64, true);
        private Datafield metadataFile = new Datafield("File", DbType.String);
        private string metaDataTable;
        private DatabaseHelper connector;

        public MetadataDatasource(string tableName)
        {
            this.connector = new SqliteHelper();
            this.metaDataTable = $"{tableName}_metadata";
            var metadataSchema = new SchemaDefinition(metaDataTable);
            metadataSchema.columns = MetaDataFields();
            metadataSchema.indexes.Add(new IndexDefinition(true, metadataID.name));
            metadataSchema.indexes.Add(new IndexDefinition(true, metadataFile.name));
            connector.CreateTable(metadataSchema);
        }

        public static List<string> FilesToLoad()
        {
            var connector = new SqliteHelper();

            var weathersDir = Path.Combine(UnityEngine.Application.dataPath, "Resources", "Weathers");
            if (!Directory.Exists(weathersDir))
            {
                throw new Exception($"Weathers folder not found: {weathersDir}");
            }

            var allFiles = Directory.EnumerateFiles(weathersDir, "*.csv", SearchOption.TopDirectoryOnly)
                .Select(f => Path.Combine("Weathers", Path.GetFileName(f)));

            var metadata = new MetadataDatasource(WeatherHistoryDatasource.tableName);
            var existingFiles = new List<string>();

            using (var reader = connector.ExecuteRawQuery($"SELECT {metadata.metadataFile.name} FROM {metadata.metaDataTable}"))
            {
                while (reader.Read())
                {
                    existingFiles.Add((string)reader.GetValue(0));
                }
            }
            return allFiles.Except(existingFiles).ToList();
        }

        public void FlagProcessed(string fileName)
        {
            connector.Insert(
                metaDataTable,
                MetaDataFields(),
                new List<string>() { "", fileName }
            );
        }

        private List<Datafield> MetaDataFields()
        {
            return new List<Datafield>()
            {
                metadataID,
                metadataFile
            };
        }
    }
}
