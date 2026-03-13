using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
    public class DimensionTableLoader : SqliteTableLoader
    {
        public IDimension Dimension => (IDimension)Table;

        public DimensionCache Cache { get; }

        private int _lookupCsvIndex = -1;

        public DimensionTableLoader(IDimension dimension) : base(dimension)
        {
            Cache = new DimensionCache(dimension);
        }

        public override void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvFieldNames)
        {
            base.StartStaging(connection, transaction, csvFieldNames);
            _lookupCsvIndex = FindCsvIndex(Dimension.LookupSourceAttribute.name, csvFieldNames);
        }

        public bool TryStageLine(string[] csvLine)
        {
            if (_lookupCsvIndex < 0 || _lookupCsvIndex >= csvLine.Length)
            {
                StageLine(csvLine);
                return true;
            }

            var lookupValue = csvLine[_lookupCsvIndex];
            if (!Cache.ShouldStage(lookupValue))
                return false;

            StageLine(csvLine);
            return true;
        }

        public override void Merge(SqliteConnection connection)
        {
            base.Merge(connection);
            Cache.LoadFromDatabase(connection);
        }
    }
}
