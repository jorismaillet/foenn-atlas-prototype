using System;
using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;

namespace Assets.Scripts.ETL.Loaders
{
    public class SqliteTableLoader : IDisposable
    {
        public Table Table { get; }

        protected SqliteCommand _stageCommand;

        protected SqliteParameterCollection _stageParams;

        protected List<Func<string[], object>> _valueResolvers;

        public SqliteTableLoader(Table table)
        {
            Table = table;
        }

        public virtual void StartStaging(SqliteConnection connection, SqliteTransaction transaction, string[] csvHeaders)
        {
            _valueResolvers = new List<Func<string[], object>>();
            foreach (var mapping in Table.Mappings)
            {
                _valueResolvers.Add(mapping.GetMappingResolver(csvHeaders));
            }
            SqliteHelper.CreateStagingTable(connection, Table);
            _stageCommand?.Dispose();
            _stageCommand = SqliteHelper.GetStageCommand(connection, Table);
            _stageParams = _stageCommand.Parameters;
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

        public void Dispose()
        {
            _stageCommand?.Dispose();
        }
    }
}
