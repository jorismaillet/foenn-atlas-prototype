using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasets;
using Assets.Scripts.Foenn.ETL.Dimensions;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Unity;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.MemoryProfiler;
using UnityEditor.SceneManagement;

namespace Assets.Scripts.Foenn.ETL
{
    public class WeatherHistoryProcessor
    {
        private string databasePath;
        private CSVExtractor extractor;
        private List<SqliteTableLoader> loaders = new List<SqliteTableLoader>();

        public WeatherHistoryProcessor(string databasePath, string fileName)
        {
            this.databasePath = databasePath;
            this.extractor = new CSVExtractor(fileName);
            var dataset = new WeatherHistoryDataset();
            foreach (var dimension in dataset.Dimensions) {
                loaders.Add(new SqliteTableLoader(dimension));
            }
            foreach (var fact in dataset.Facts) {
                loaders.Add(new SqliteTableLoader(fact));
            }
        }

        private int loaded = 0, inBatch = 0, batchSize = 10000;
        public void ProcessETL(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fieldNames = extractor.ExtractFieldNames();
            var stageConnection = SqliteHelper.CreateConnection(databasePath);
            SqliteHelper.ApplyStagingPragmas(stageConnection);
            var transaction = stageConnection.BeginTransaction();
            foreach (var loader in loaders) {
                loader.StartStaging(stageConnection, transaction, fieldNames);
            }
            foreach (var line in extractor.ExtractValues())
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var loader in loaders) {
                    loader.StageLine(line);
                    loaded++;
                    inBatch++;
                }
                if (inBatch >= batchSize) {
                    transaction.Commit();
                    MainThreadLog.Log($"Inserted batch, total={loaded}");
                    transaction.Dispose();
                    transaction = stageConnection.BeginTransaction();
                    inBatch = 0;
                }
            }
            transaction.Commit();
            stageConnection.Dispose();
            MainThreadLog.Log($"Finished staging, total={loaded}");
            transaction.Dispose();
            var mergeConnection = SqliteHelper.CreateConnection(databasePath);
            transaction = mergeConnection.BeginTransaction();
            foreach (var loader in loaders) {
                SqliteHelper.MergeStagingTable(mergeConnection, loader.table);
            }
            transaction.Commit();
            MainThreadLog.Log($"Finished merged");
            mergeConnection.Dispose();
        }
    }
}