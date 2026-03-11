namespace Assets.Scripts.Foenn.ETL
{
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Mono.Data.Sqlite;
    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    public class SqliteTableLoader
    {
        public ITable Table { get; }

        protected SqliteCommand _stageCommand;
        protected SqliteParameter[] _stageParams;
        protected Func<string, object>[] _converters;
        protected int[] _csvToColumnMap;

        public SqliteTableLoader(ITable table)
        {
            Table = table;
        }

        public virtual void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvFieldNames)
        {
            var columns = Table.Columns.Where(c => !(c is PrimaryKey pk && pk.autoIncrement)).ToList();

            _stageCommand = connection.CreateCommand();
            _stageCommand.Transaction = transaction;
            var colNames = string.Join(", ", columns.Select(c => c.name));
            var paramNames = string.Join(", ", columns.Select(c => $"@{c.name}"));
            _stageCommand.CommandText = $"INSERT INTO \"{Table.Name}_staging\" ({colNames}) VALUES ({paramNames})";

            _stageParams = new SqliteParameter[columns.Count];
            _converters = new Func<string, object>[columns.Count];
            _csvToColumnMap = new int[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                _stageParams[i] = new SqliteParameter($"@{col.name}", col.dbType);
                _stageCommand.Parameters.Add(_stageParams[i]);
                _converters[i] = GetConverter(col.dbType);
                _csvToColumnMap[i] = FindCsvIndex(col.name, csvFieldNames);
            }
        }

        public virtual void StageLine(string[] csvLine)
        {
            for (int i = 0; i < _stageParams.Length; i++)
            {
                int csvIdx = _csvToColumnMap[i];
                string rawValue = csvIdx >= 0 && csvIdx < csvLine.Length ? csvLine[csvIdx] : string.Empty;
                _stageParams[i].Value = _converters[i](rawValue);
            }
            _stageCommand.ExecuteNonQuery();
        }

        public virtual void Merge(SqliteConnection connection)
        {
            SqliteHelper.MergeStagingTable(connection, Table);
            SqliteHelper.DropStagingTable(connection, Table);
        }

        protected int FindCsvIndex(string fieldName, string[] csvFieldNames)
        {
            for (int i = 0; i < csvFieldNames.Length; i++)
            {
                if (csvFieldNames[i].Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        protected Func<string, object> GetConverter(DbType dbType)
        {
            return dbType switch
            {
                DbType.String => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)s,
                DbType.Single or DbType.Double => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)double.Parse(s, CultureInfo.InvariantCulture),
                DbType.Int64 => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)long.Parse(s, CultureInfo.InvariantCulture),
                DbType.Int32 => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)int.Parse(s, CultureInfo.InvariantCulture),
                DbType.Int16 => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)short.Parse(s, CultureInfo.InvariantCulture),
                _ => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)s
            };
        }

        public void Dispose()
        {
            _stageCommand?.Dispose();
        }
    }
}
