namespace Assets.Scripts.Foenn.ETL
{
    using Assets.Scripts.Foenn.Engine.Connectors;
    using Assets.Scripts.Foenn.ETL.Datasets;
    using Assets.Scripts.Foenn.ETL.Extractors;
    using Assets.Scripts.Foenn.ETL.Loaders;
    using Assets.Scripts.Unity;
    using System.Collections.Generic;
    using System.Threading;

    public class WeatherHistoryProcessor
    {
        private CSVExtractor extractor;

        private List<SqliteTableLoader> loaders = new List<SqliteTableLoader>();

        public WeatherHistoryProcessor(string fileName)
        {
            this.extractor = new CSVExtractor(fileName);
            foreach (var dimension in WeatherHistoryDataset.Dimensions)
            {
                loaders.Add(new SqliteTableLoader(dimension));
            }
            foreach (var fact in WeatherHistoryDataset.Facts)
            {
                loaders.Add(new SqliteTableLoader(fact));
            }
        }

        private int loaded = 0, inBatch = 0, batchSize = 10000;

        public void ProcessETL(CancellationToken cancellationToken = default)
        {
            using (var stageConnection = SqliteHelper.CreateConnection())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fieldNames = extractor.ExtractFieldNames();
                SqliteHelper.ApplyStagingPragmas(stageConnection);
                var transaction = stageConnection.BeginTransaction();
                foreach (var loader in loaders)
                {
                    loader.StartStaging(stageConnection, transaction, fieldNames);
                }
                foreach (var line in extractor.ExtractValues())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    foreach (var loader in loaders)
                    {
                        loader.StageLine(line);
                        loaded++;
                        inBatch++;
                    }
                    if (inBatch >= batchSize)
                    {
                        transaction.Commit();
                        MainThreadLog.Log($"Inserted batch, total={loaded}");
                        transaction.Dispose();
                        transaction = stageConnection.BeginTransaction();
                        inBatch = 0;
                    }
                }
                transaction.Commit();
                stageConnection.Dispose();
                transaction.Dispose();
            }
            MainThreadLog.Log($"Finished staging, total={loaded}");
            using (var mergeConnection = SqliteHelper.CreateConnection())
            {
                var transaction = mergeConnection.BeginTransaction();
                foreach (var loader in loaders)
                {
                    SqliteHelper.MergeStagingTable(mergeConnection, loader.table);
                }
                MainThreadLog.Log($"Finished merged");
                transaction.Commit();
                mergeConnection.Dispose();
            }
        }
    }
}
