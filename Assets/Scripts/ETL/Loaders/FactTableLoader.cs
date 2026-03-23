using System.Collections.Generic;
using Assets.Scripts.Helpers;
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

                var lookupValueResolver = dimension.LookupFieldMap.GetMappingResolver(csvFieldNames);

                _valueResolvers.Add(line =>
                {
                    try
                    {
                        return cache.Get(lookupValueResolver(line));
                    }
                    catch (KeyNotFoundException)
                    {
                        Debug.LogError($"Lookup value not found in dimension '{dimension.Name}' for column '{dimension.LookupSourceAttribute.name}'");
                        throw;
                    }
                });
            }
        }
    }
}
