using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Components.Logger;
using Assets.Scripts.Database;
using Assets.Scripts.ETL.Extractors;
using Assets.Scripts.ETL.Loaders;
using Assets.Scripts.OLAP.Datasets;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL
{
    public class ETLProcessor
    {
        private CSVExtractor _extractor;

        private List<DimensionTableLoader> _dimensionLoaders = new List<DimensionTableLoader>();

        private List<FactTableLoader> _factLoaders = new List<FactTableLoader>();

        private List<Fact> derivedFacts = new List<Fact>();

        public ETLProcessor(string csvFileName, Dataset dataset)
        {
            _extractor = new CSVExtractor(csvFileName);

            foreach (var dimension in dataset.Dimensions)
                _dimensionLoaders.Add(new DimensionTableLoader(dimension));

            foreach (var fact in dataset.Facts)
                _factLoaders.Add(new FactTableLoader(fact));
            derivedFacts = dataset.DerivedFacts;
        }

        private int _loaded = 0, _inBatch = 0, _batchSize = 50000;

        public void ProcessETL(SqliteConnection connection, CancellationToken cancellationToken = default)
        {
            try
            {
                var fieldNames = _extractor.ExtractFieldNames();
                SqliteHelper.ApplyStagingPragmas(connection);

                StageDimensions(connection, fieldNames, cancellationToken);
                MergeDimensions(connection);

                var dimensionCaches = BuildDimensionCaches();

                StageFacts(connection, fieldNames, dimensionCaches, cancellationToken);
                MergeFacts(connection);

                DeriveFacts(connection, dimensionCaches);

                MainThreadLog.Log($"Finished ETL, total records={_loaded}");
            }
            finally
            {
                foreach (var loader in _dimensionLoaders)
                    loader.Dispose();

                foreach (var loader in _factLoaders)
                    loader.Dispose();
            }
        }

        private void StageDimensions(SqliteConnection connection, string[] fieldNames, CancellationToken ct)
        {
            var transaction = connection.BeginTransaction();

            try
            {
                foreach (var loader in _dimensionLoaders)
                {
                    loader.StartStaging(connection, transaction, fieldNames);
                    loader.StartBatch(transaction);
                }

                foreach (var line in _extractor.ExtractValues())
                {
                    ct.ThrowIfCancellationRequested();

                    foreach (var loader in _dimensionLoaders)
                        if (loader.TryStageLine(line))
                        {
                            _loaded++;
                            _inBatch++;
                        }

                    if (_inBatch >= _batchSize)
                    {
                        transaction.Commit();
                        MainThreadLog.Log($"Staged dimensions batch, total={_loaded}");
                        transaction.Dispose();
                        transaction = connection.BeginTransaction();

                        foreach (var loader in _dimensionLoaders)
                            loader.StartBatch(transaction);

                        _inBatch = 0;
                    }
                }

                transaction.Commit();
                MainThreadLog.Log($"Staged dimensions batch, total={_loaded}");
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        private void MergeDimensions(SqliteConnection connection)
        {
            foreach (var loader in _dimensionLoaders)
                loader.Merge(connection);

            MainThreadLog.Log("Merged dimensions");
        }

        private Dictionary<Dimension, DimensionCache> BuildDimensionCaches()
        {
            var caches = new Dictionary<Dimension, DimensionCache>();
            foreach (var loader in _dimensionLoaders)
                caches[loader.Dimension] = loader.Cache;
            return caches;
        }

        private void StageFacts(SqliteConnection connection, string[] fieldNames, Dictionary<Dimension, DimensionCache> caches, CancellationToken ct)
        {
            _loaded = 0;
            _inBatch = 0;

            var transaction = connection.BeginTransaction();

            try
            {
                foreach (var loader in _dimensionLoaders)
                {
                    loader.Cache.LoadFromDatabase(connection);
                }
                foreach (var loader in _factLoaders)
                {
                    loader.StartStaging(connection, transaction, fieldNames, caches);
                    loader.StartBatch(transaction);
                }

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
                            loader.StartBatch(transaction);

                        _inBatch = 0;
                    }
                }

                transaction.Commit();
                MainThreadLog.Log($"Staged facts batch, total={_loaded}");
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        private void MergeFacts(SqliteConnection connection)
        {
            foreach (var loader in _factLoaders)
                loader.Merge(connection);

            MainThreadLog.Log("Merged facts");
        }

        private void DeriveFacts(SqliteConnection connection, Dictionary<Dimension, DimensionCache> caches)
        {
            foreach (var derived in derivedFacts)
            {
                var transaction = connection.BeginTransaction();
                derived.BuildDerivedFact(connection, caches);
                transaction.Commit();
                transaction.Dispose();
                MainThreadLog.Log($"Derived fact {derived.Name}");
            }
        }
    }
}
