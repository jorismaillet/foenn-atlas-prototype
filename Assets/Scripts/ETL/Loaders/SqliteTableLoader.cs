using Assets.Scripts.Helpers;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.ETL.Loaders
{
    public class SqliteTableLoader : IDisposable
    {
        public Table Table { get; }

        protected SqliteCommand _stageCommand;

        protected SqliteParameterCollection _stageParams;

        protected List<Func<string[], object>> _valueResolvers;

        private Func<string[], object>[] _resolverArray;

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

        public void StartBatch(SqliteTransaction transaction)
        {
            _stageCommand.Transaction = transaction;
            _resolverArray = _valueResolvers.ToArray();
        }

        public void StageLine(string[] csvLine)
        {
            for (int i = 0; i < csvLine.Length; i++)
            {
                if (!string.IsNullOrEmpty(csvLine[i]))
                    goto stage;
            }
            return;

        stage:
            var resolvers = _resolverArray;
            for (int i = 0; i < resolvers.Length; i++)
            {
                _stageParams[i].Value = resolvers[i](csvLine);
            }
            _stageCommand.ExecuteNonQuery();
        }

        public void Merge(SqliteConnection connection)
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
