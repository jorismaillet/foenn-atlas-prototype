namespace Assets.Scripts.Foenn.ETL
{
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Mono.Data.Sqlite;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    public class SqliteTableLoader
    {
        public ITable Table { get; }

        protected SqliteCommand _stageCommand;
        protected SqliteParameter[] _stageParams;
        protected List<Func<string[], object>> _valueResolvers;

        public SqliteTableLoader(ITable table)
        {
            Table = table;
        }

        public virtual void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvFieldNames)
        {
            var columns = Table.Columns.Where(c => !c.autoIncrement).ToList();
            var mappings = Table.Mappings;

            _stageCommand = connection.CreateCommand();
            _stageCommand.Transaction = transaction;
            var colNames = string.Join(", ", columns.Select(c => c.name));
            var paramNames = string.Join(", ", columns.Select(c => $"@{c.name}"));
            _stageCommand.CommandText = $"INSERT INTO \"{Table.TableName}_staging\" ({colNames}) VALUES ({paramNames})";

            _stageParams = new SqliteParameter[columns.Count];
            _valueResolvers = new List<Func<string[], object>>(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                _stageParams[i] = new SqliteParameter($"@{col.name}", col.dbType);
                _stageCommand.Parameters.Add(_stageParams[i]);

                var mapping = mappings.FirstOrDefault(m => m.targetField == col);
                if (mapping != null)
                {
                    int csvIdx = FindCsvIndex(mapping.csvColumn, csvFieldNames);
                    if (mapping.transform != null)
                    {
                        _valueResolvers.Add(line => {
                            var raw = csvIdx >= 0 ? line[csvIdx] : string.Empty;
                            if (string.IsNullOrEmpty(raw)) return DBNull.Value;
                            return mapping.transform(raw);
                        });
                    }
                    else
                    {
                        var converter = GetConverter(col.dbType);
                        _valueResolvers.Add(line => {
                            var raw = csvIdx >= 0 ? line[csvIdx] : string.Empty;
                            return converter(raw);
                        });
                    }
                }
                else
                {
                    _valueResolvers.Add(_ => DBNull.Value);
                }
            }
        }

        public virtual void StageLine(string[] csvLine)
        {
            for (int i = 0; i < _stageParams.Length; i++)
                _stageParams[i].Value = _valueResolvers[i](csvLine);

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
