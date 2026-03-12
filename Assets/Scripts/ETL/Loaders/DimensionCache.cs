using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
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
            var sql = $"SELECT \"{_dimension.PrimaryKey.name}\" FROM \"{_dimension.name}\"";
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
