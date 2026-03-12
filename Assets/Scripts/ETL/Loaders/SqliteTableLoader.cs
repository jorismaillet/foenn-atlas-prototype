using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
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
            SqliteHelper.CreateStagingTable(connection, Table);

            var mappings = Table.Mappings;

            _stageCommand = connection.CreateCommand();
            _stageCommand.Transaction = transaction;
            var colNames = string.Join(", ", mappings.Select(c => c.targetField.name));
            var paramNames = string.Join(", ", mappings.Select(c => $"@{c.targetField.name}"));
            _stageCommand.CommandText = $"INSERT INTO \"{Table.name}_staging\" ({colNames}) VALUES ({paramNames})";

            _stageParams = new SqliteParameter[mappings.Count];
            _valueResolvers = new List<Func<string[], object>>(mappings.Count);

            for (int i = 0; i < mappings.Count; i++)
            {
                var mapping = mappings[i];
                _stageParams[i] = new SqliteParameter($"@{mapping.targetField.name}", mapping.targetField.dbType);
                _stageCommand.Parameters.Add(_stageParams[i]);
                int csvIdx = FindCsvIndex(mapping.column.name, csvFieldNames);
                var converter = mapping.column.GetConverter();
                if (mapping.transform != null)
                {
                    _valueResolvers.Add(line =>
                    {
                        var raw = csvIdx >= 0 ? line[csvIdx] : string.Empty;
                        if (string.IsNullOrEmpty(raw)) return DBNull.Value;
                        var transformed = mapping.transform(raw);
                        return converter(transformed);
                    });
                }
                else
                {
                    _valueResolvers.Add(line =>
                    {
                        var raw = csvIdx >= 0 ? line[csvIdx] : string.Empty;
                        return converter(raw);
                    });
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

        public void Dispose()
        {
            _stageCommand?.Dispose();
        }
    }
}
