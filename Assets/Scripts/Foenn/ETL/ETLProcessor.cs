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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEditor.MemoryProfiler;

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

        public void ProcessETL(CancellationToken cancellationToken = default)
        {

            var swRead = new Stopwatch();
            var swTransform = new Stopwatch();
            var swLoad = new Stopwatch();
            long n = 0;



            MainThreadLog.Log("Start");
            cancellationToken.ThrowIfCancellationRequested();
            var newProcess = ShouldProcess();
            MainThreadLog.Log("Extract");
            var schema = new SchemaDefinition(datasource.TableName());
            schema.AddColumns(extractor.ExtractHeaders());
            var headersCount = schema.columns.Count;
            datasource.PrepareTranformer(schema);
            transformer.TransformColumns(schema);
            MainThreadLog.Log("Create table");
            loader.Connector().CreateStagingTable(schema);
            try
            {
                MainThreadLog.Log("StartLoad");
                loader.StartStaging(schema);
                MainThreadLog.Log("ExtractContent");
                foreach (var line in extractor.ExtractContent(headersCount))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    transformer.TransformLine(schema, line);
                    loader.StageLine(line, datasource.GetExtraColumns(line));
                    n++;
                }
                cancellationToken.ThrowIfCancellationRequested();
                loader.CommitStaging();
                loader.Connector().CreateTable(schema);
                loader.MergeStaging(schema);

                if (newProcess) FlagProcessed(loader);
            }
            finally
            {
                loader.Connector().DropStagingTable(schema);
                loader.Connector().CloseSession();
            }
            MainThreadLog.Log("End");
        }

        private PrimaryKey metadataID = new PrimaryKey("ID", DbType.Int64, true);
        private Datafield metadataFile = new Datafield("File", DbType.String);

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
            metadataSchema.primaryKey = metadataID;
            metadataSchema.indexes.Add(metadataID);
            metadataSchema.columns = MetaDataFields();
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