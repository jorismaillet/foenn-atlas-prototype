using System;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
    public class DimensionTableLoader : SqliteTableLoader
    {
        public Dimension Dimension => (Dimension)Table;

        public DimensionCache Cache { get; }

        private int _lookupCsvIndex = -1;

        public DimensionTableLoader(Dimension dimension) : base(dimension)
        {
            Cache = new DimensionCache(dimension);
        }

        public override void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvHeaders)
        {
            base.StartStaging(connection, transaction, csvHeaders);
            _lookupCsvIndex = FindCsvIndex(Dimension.LookupSourceAttribute.name, csvHeaders);
            if (_lookupCsvIndex == -1)
            {
                throw new Exception("Lookup column not found in csv headers for dimension " + Dimension.Name);
            }
        }

        public bool TryStageLine(string[] csvLine)
        {
            var lookupValue = csvLine[_lookupCsvIndex];
            if (!Cache.ShouldStage(lookupValue))
                return false;
            StageLine(csvLine);
            return true;
        }
    }
}
