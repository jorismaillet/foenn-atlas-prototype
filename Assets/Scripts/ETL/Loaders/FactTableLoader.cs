using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Assets.Scripts.ETL.Loaders
{
    public class FactTableLoader : SqliteTableLoader
    {
        public Fact Fact;

        public FactTableLoader(Fact fact) : base(fact)
        {
            Fact = (Fact)Table;
        }

        public void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvFieldNames, Dictionary<Dimension, DimensionCache> caches)
        {
            base.StartStaging(connection, transaction, csvFieldNames);

            foreach (var dimension in Fact.dimensions)
            {
                var cache = caches[dimension];
                string lookupCsvColumn = dimension.LookupSourceAttribute.name;
                int csvIdx = FindCsvIndex(lookupCsvColumn, csvFieldNames);
                _valueResolvers.Add(line =>
                {
                    try
                    {
                        return cache.Get(line[csvIdx]);
                    }
                    catch (KeyNotFoundException)
                    {
                        Debug.LogError($"Value '{line[csvIdx]}' not found in dimension '{dimension.Name}' for column '{lookupCsvColumn}'");
                        throw;
                    }
                });
            }
        }
    }
}
