using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;
using System.Collections.Generic;

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
                    return cache.Get(lookupValueResolver(line));
                });
            }
        }
    }
}
