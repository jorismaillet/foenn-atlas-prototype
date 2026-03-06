using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
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
        public Loader loader;

        public ETLProcessor(Datasource datasource, Extractor extractor, Loader loader)
        {
            this.datasource = datasource;
            this.extractor = extractor;
            this.loader = loader;
        }

        public void ProcessETL(CancellationToken cancellationToken = default)
        {
            long n = 0;
            cancellationToken.ThrowIfCancellationRequested();
            var schema = new SchemaDefinition(datasource.TableName());
            schema.AddColumns(extractor.ExtractHeaders());
            var headersCount = schema.columns.Count;
            datasource.PrepareSchema(schema);
            loader.Connector().CreateStagingTable(schema);

            bool stagingStarted = false;
            bool stagingEnded = false;
            try
            {
                loader.StartStaging(schema);
                stagingStarted = true;
                foreach (var line in extractor.ExtractContent(headersCount))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    loader.StageLine(line, datasource.GetExtraColumns(line));
                    n++;
                }
                cancellationToken.ThrowIfCancellationRequested();
                loader.EndStaging();
                stagingEnded = true;

                loader.Connector().CreateTable(schema);
                loader.MergeStaging(schema);
            }
            finally
            {
                if (stagingStarted && !stagingEnded)
                {
                    try { loader.EndStaging(); }
                    catch {}
                }
                loader.Connector().DropStagingTable(schema);
                loader.Connector().CloseSession();
            }
        }
    }
}