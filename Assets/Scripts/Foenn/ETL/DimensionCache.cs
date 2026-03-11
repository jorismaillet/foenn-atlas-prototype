namespace Assets.Scripts.Foenn.ETL
{
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Mono.Data.Sqlite;
    using System.Collections.Generic;

    public class DimensionCache
    {
        private readonly IDimension _dimension;
        private readonly Dictionary<string, int> _cache = new Dictionary<string, int>();

        public DimensionCache(IDimension dimension)
        {
            _dimension = dimension;
        }

        public void LoadFromDatabase(SqliteConnection connection)
        {
            _cache.Clear();
            var sql = $"SELECT \"{_dimension.PrimaryKey.name}\", \"{_dimension.LookupKey.name}\" FROM \"{_dimension.Name}\"";
            using (var reader = SqliteHelper.ExecuteReader(connection, sql))
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var lookupValue = reader.GetValue(1)?.ToString() ?? string.Empty;
                    _cache[lookupValue] = id;
                }
            }
        }

        public bool TryGetId(string lookupValue, out int id)
        {
            return _cache.TryGetValue(lookupValue, out id);
        }

        public int Count => _cache.Count;
    }
}
