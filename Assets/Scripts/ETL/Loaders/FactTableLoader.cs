using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

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

            var columnsWithoutPk = Fact.Columns.Where(c => !c.autoIncrement).ToList();

            foreach (var refField in Fact.References)
            {
                if (caches.TryGetValue(refField.referencedDimension, out var cache))
                {
                    int sourceIdx = FindCsvIndex(refField.sourceCsvColumn, csvFieldNames);
                    int colIndex = columnsWithoutPk.FindIndex(c => c.name == refField.name);

                    if (colIndex >= 0)
                    {
                        _valueResolvers[colIndex] = line =>
                        {
                            string lookupValue = sourceIdx >= 0 ? line[sourceIdx] : string.Empty;
                            return cache.TryGetId(lookupValue, out int dimId) ? dimId : (object)System.DBNull.Value;
                        };
                    }
                }
            }
        }
    }
}
