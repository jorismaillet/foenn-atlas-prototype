namespace Assets.Scripts.Foenn.ETL
{
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Assets.Scripts.Unity;
    using System.Collections.Generic;
    using System.Threading;

    public class ETLProcessor
    {
        private CSVExtractor _extractor;
        private List<DimensionTableLoader> _dimensionLoaders = new List<DimensionTableLoader>();
        private List<FactTableLoader> _factLoaders = new List<FactTableLoader>();

        public ETLProcessor(string csvFileName, List<IDimension> dimensions, List<IFact> facts)
        {
            _extractor = new CSVExtractor(csvFileName);

            foreach (var dimension in dimensions)
                _dimensionLoaders.Add(new DimensionTableLoader(dimension));

            foreach (var fact in facts)
                _factLoaders.Add(new FactTableLoader(fact));
        }

        private int _loaded = 0, _inBatch = 0, _batchSize = 10000;

        public void ProcessETL(CancellationToken cancellationToken = default)
        {
            var fieldNames = _extractor.ExtractFieldNames();

            using (var connection = SqliteHelper.CreateConnection())
            {
                SqliteHelper.ApplyStagingPragmas(connection);

                StageDimensions(connection, fieldNames, cancellationToken);
                MergeDimensions(connection);

                var dimensionCaches = BuildDimensionCaches();
                StageFacts(connection, fieldNames, dimensionCaches, cancellationToken);
                MergeFacts(connection);
            }

            MainThreadLog.Log($"Finished ETL, total records={_loaded}");
        }

        private void StageDimensions(Mono.Data.Sqlite.SqliteConnection connection, string[] fieldNames, CancellationToken ct)
        {
            var transaction = connection.BeginTransaction();

            foreach (var loader in _dimensionLoaders)
                loader.StartStaging(connection, transaction, fieldNames);

            foreach (var line in _extractor.ExtractValues())
            {
                ct.ThrowIfCancellationRequested();

                foreach (var loader in _dimensionLoaders)
                    loader.StageLine(line);

                _loaded++;
                _inBatch++;

                if (_inBatch >= _batchSize)
                {
                    transaction.Commit();
                    MainThreadLog.Log($"Staged dimensions batch, total={_loaded}");
                    transaction.Dispose();
                    transaction = connection.BeginTransaction();

                    foreach (var loader in _dimensionLoaders)
                        loader.StartStaging(connection, transaction, fieldNames);

                    _inBatch = 0;
                }
            }

            transaction.Commit();
            transaction.Dispose();
        }

        private void MergeDimensions(Mono.Data.Sqlite.SqliteConnection connection)
        {
            foreach (var loader in _dimensionLoaders)
                loader.Merge(connection);

            MainThreadLog.Log("Merged dimensions");
        }

        private Dictionary<IDimension, DimensionCache> BuildDimensionCaches()
        {
            var caches = new Dictionary<IDimension, DimensionCache>();
            foreach (var loader in _dimensionLoaders)
                caches[loader.Dimension] = loader.Cache;
            return caches;
        }

        private void StageFacts(Mono.Data.Sqlite.SqliteConnection connection, string[] fieldNames, Dictionary<IDimension, DimensionCache> caches, CancellationToken ct)
        {
            _loaded = 0;
            _inBatch = 0;

            var transaction = connection.BeginTransaction();

            foreach (var loader in _factLoaders)
                loader.StartStaging(connection, transaction, fieldNames, caches);

            foreach (var line in _extractor.ExtractValues())
            {
                ct.ThrowIfCancellationRequested();

                foreach (var loader in _factLoaders)
                    loader.StageLine(line);

                _loaded++;
                _inBatch++;

                if (_inBatch >= _batchSize)
                {
                    transaction.Commit();
                    MainThreadLog.Log($"Staged facts batch, total={_loaded}");
                    transaction.Dispose();
                    transaction = connection.BeginTransaction();

                    foreach (var loader in _factLoaders)
                        loader.StartStaging(connection, transaction, fieldNames, caches);

                    _inBatch = 0;
                }
            }

            transaction.Commit();
            transaction.Dispose();
        }

        private void MergeFacts(Mono.Data.Sqlite.SqliteConnection connection)
        {
            foreach (var loader in _factLoaders)
                loader.Merge(connection);

            MainThreadLog.Log("Merged facts");
        }
    }
}
