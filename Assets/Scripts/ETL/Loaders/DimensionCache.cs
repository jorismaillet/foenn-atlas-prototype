using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;
using SqlKata;

namespace Assets.Scripts.ETL.Loaders
{
    public class DimensionCache
    {
        public readonly Dimension _dimension;

        private readonly Dictionary<string, int> _cache = new Dictionary<string, int>();

        private readonly HashSet<string> _stagedLookupValues = new HashSet<string>();

        public DimensionCache(Dimension dimension)
        {
            _dimension = dimension;
        }

        public void LoadFromDatabase(SqliteConnection connection)
        {
            _cache.Clear();
            using (var reader = SqliteHelper.ExecuteReader(connection, new Query(_dimension.Name).Select(_dimension.PrimaryKey.fieldName, _dimension.LookupField.fieldName)))
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var lookupValue = reader.GetValue(1).ToString();
                    _cache[lookupValue] = id;
                }
            }
        }

        public bool ShouldStage(string lookupValue)
        {
            lookupValue = lookupValue ?? string.Empty;
            if (_cache.ContainsKey(lookupValue))
                return false;

            return _stagedLookupValues.Add(lookupValue);
        }

        public int Get(string lookupValue)
        {
            return _cache[lookupValue];
        }
    }
}
