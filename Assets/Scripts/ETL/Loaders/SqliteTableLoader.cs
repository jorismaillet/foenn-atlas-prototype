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
            var insertFields = mappings.Select(c => c.targetField).Concat(Table.References).ToList();

            _stageCommand?.Dispose();

            _stageCommand = connection.CreateCommand();
            var colNames = string.Join(", ", insertFields.Select(c => c.name));
            var paramNames = string.Join(", ", insertFields.Select(c => $"@{c.name}"));
            _stageCommand.CommandText = $"INSERT INTO \"{Table.name}_staging\" ({colNames}) VALUES ({paramNames})";

            _stageParams = new SqliteParameter[mappings.Count + Table.References.Count];
            _valueResolvers = new List<Func<string[], object>>();

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
                        var raw = line[csvIdx];
                        if (string.IsNullOrEmpty(raw)) return DBNull.Value;
                        var transformed = mapping.transform(raw);
                        return converter(transformed);
                    });
                }
                else
                {
                    _valueResolvers.Add(line =>
                    {
                        return converter(line[csvIdx]);
                    });
                }
            }
        }

        public virtual void StartBatch(SqliteTransaction transaction)
        {
            _stageCommand.Transaction = transaction;
        }

        public virtual void StageLine(string[] csvLine)
        {
            bool allNullValues = true;
            for (int i = 0; i < _stageParams.Length; i++)
            {
                var value = _valueResolvers[i](csvLine);
                _stageParams[i].Value = value;

                if (value != null && value != DBNull.Value)
                    allNullValues = false;
            }

            if (allNullValues)
                return;

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
