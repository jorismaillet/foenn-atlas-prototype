using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
        private SqlConnector connector;

        public MetadataDatasource(string tableName)
        {
            this.connector = new SqliteConnector();
            this.metaDataTable = $"{tableName}_metadata";
            var metadataSchema = new SchemaDefinition(metaDataTable);
            metadataSchema.columns = MetaDataFields();
            metadataSchema.indexes.Add(new IndexDefinition(true, metadataID.name));
            metadataSchema.indexes.Add(new IndexDefinition(true, metadataFile.name));
            connector.CreateTable(metadataSchema);
        }

        public List<string> FilesToLoad(List<string> allFiles)
        {
            using (var reader = connector.ExecuteRawQuery($"SELECT {metadataFile.name} FROM {metaDataTable}"))
            {
                var existingFiles = new List<string>();
                while (reader.Read())
                {
                    existingFiles.Add((string)reader.GetValue(0));
                }
                return allFiles.Except(existingFiles).ToList();
            }
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
