using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
    public class DimensionTableLoader : SqliteTableLoader
    {
        public IDimension Dimension => (IDimension)Table;

        public DimensionCache Cache { get; }

        public DimensionTableLoader(IDimension dimension) : base(dimension)
        {
            Cache = new DimensionCache(dimension);
        }

        public override void Merge(SqliteConnection connection)
        {
            base.Merge(connection);
            Cache.LoadFromDatabase(connection);
        }
    }
}
