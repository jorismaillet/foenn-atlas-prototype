using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
    public class SqliteTableLoader
    {
        public ITable Table { get; }

        protected SqliteCommand _stageCommand;

        protected SqliteParameterCollection _stageParams;

        protected List<Func<string[], object>> _valueResolvers;

        public SqliteTableLoader(ITable table)
        {
            Table = table;
        }

        public virtual void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvFieldNames)
        {
            SqliteHelper.CreateStagingTable(connection, Table);
            var mappings = Table.Mappings;
            _stageCommand?.Dispose();
            _stageCommand = SqliteHelper.GetStageCommand(connection, Table);
            _stageParams = _stageCommand.Parameters;
            _valueResolvers = new List<Func<string[], object>>();
            foreach (var mapping in mappings)
            {
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
            for (int i = 0; i < _stageParams.Count; i++)
            {
                var value = _valueResolvers[i](csvLine);
                _stageParams[i].Value = value;

                if (value != null && value != DBNull.Value)
                    allNullValues = false;
            }
            if (!allNullValues)
                _stageCommand.ExecuteNonQuery();
        }

        public virtual void Merge(SqliteConnection connection)
        {
            SqliteHelper.MergeStagingTable(connection, Table);
            SqliteHelper.DropStagingTable(connection, Table);
        }

        protected int FindCsvIndex(string fieldName, string[] csvFieldNames)
        {
            return Array.FindIndex(csvFieldNames, name => name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }

        public void Dispose()
        {
            _stageCommand?.Dispose();
        }
    }
}
