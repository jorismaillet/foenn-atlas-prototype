using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Assets.Scripts.ETL.Loaders
{
    public class FactTableLoader : SqliteTableLoader
    {
        public IFact Fact => (IFact)Table;

        public FactTableLoader(IFact fact) : base(fact)
        {
        }

        public void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvFieldNames, Dictionary<IDimension, DimensionCache> caches)
        {
            base.StartStaging(connection, transaction, csvFieldNames);

            for (int i = 0; i < Fact.References.Count; i++)
            {
                var refField = Fact.References[i];
                var cache = caches[refField.referencedDimension];
                int colIndex = Fact.Mappings.Count + i;
                string lookupCsvColumn = refField.referencedDimension.LookupSourceAttribute.name;
                int csvIdx = FindCsvIndex(lookupCsvColumn, csvFieldNames);
                _valueResolvers.Add(line =>
                {
                    try
                    {
                        return cache.Get(line[csvIdx]);
                    }
                    catch (KeyNotFoundException)
                    {
                        Debug.LogError($"Value '{line[csvIdx]}' not found in dimension '{refField.referencedDimension.name}' for column '{lookupCsvColumn}'");
                        throw;
                    }
                });
                _stageParams[colIndex] = new SqliteParameter($"@{refField.name}", refField.dbType);
                _stageCommand.Parameters.Add(_stageParams[colIndex]);
            }
        }
    }
}
