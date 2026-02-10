using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.ETL.Transformers;
using Assets.Scripts.Unity;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL
{
    public class ETLProcessor
    {

        public Datasource datasource;
        public Extractor extractor;
        public Transformer transformer;
        public Loader loader;
        private string metaDataTable;

        public ETLProcessor(Datasource datasource, Extractor extractor, Transformer transformer, Loader loader)
        {
            this.datasource = datasource;
            this.extractor = extractor;
            this.transformer = transformer;
            this.loader = loader;
            this.metaDataTable = $"{datasource.TableName()}_metadata";
        }

        public void ProcessETL()
        {
            MainThreadLog.Log("Start");
            var newProcess = ShouldProcess();
            MainThreadLog.Log("Extract");
            var schema = new SchemaDefinition(datasource.TableName());
            schema.AddHeaders(extractor.ExtractHeaders());
            transformer.TransformHeaders(schema);
            MainThreadLog.Log("Create table");
            loader.Connector().CreateTable(schema);
            try
            {
                loader.StartLoad(schema);
                foreach (var line in extractor.ExtractContent())
                {
                    transformer.TransformLine(schema, line);
                    loader.LoadLine(line);
                }
                if (newProcess) FlagProcessed(loader);
            }
            finally
            {
                loader.EndLoad();
            }
        }

        private Datafield metadataID = new Datafield("ID", Datatype.PRIMARY_KEY);
        private Datafield metadataFile = new Datafield("File", Datatype.STRING);

        private void FlagProcessed(Loader loader)
        {
            loader.Connector().Insert(
                metaDataTable,
                MetaDataFields(),
                new List<string>() {"", extractor.ExtractionID() 
            });
        }

        public bool ShouldProcess()
        {
            var metadataSchema = new SchemaDefinition(metaDataTable);
            metadataSchema.headers = MetaDataFields();
            var connector = loader.Connector();
            connector.CreateTable(metadataSchema);
            var res = !connector.Exists(metaDataTable, metadataFile.name, extractor.ExtractionID());
            return res;
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