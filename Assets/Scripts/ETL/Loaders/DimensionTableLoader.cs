using System;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
    public class DimensionTableLoader : SqliteTableLoader
    {
        public Dimension Dimension { get; }

        public DimensionCache Cache { get; }

        private Func<string[], object> _lookupValueResolver;

        public DimensionTableLoader(Dimension dimension) : base(dimension)
        {
            Dimension = (Dimension)Table;
            Cache = new DimensionCache(dimension);
        }

        public override void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvHeaders)
        {
            base.StartStaging(connection, transaction, csvHeaders);
            _lookupValueResolver = Dimension.LookupFieldMap.GetMappingResolver(csvHeaders);
        }

        public bool TryStageLine(string[] csvLine)
        {
            var lookupValue = _lookupValueResolver(csvLine);
            if (!Cache.ShouldStage(lookupValue))
                return false;
            StageLine(csvLine);
            return true;
        }
    }
}
