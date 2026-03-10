namespace Assets.Scripts.Foenn.ETL.Loaders
{
    using Assets.Scripts.Foenn.Datasets;
    using Assets.Scripts.Foenn.Engine.Connectors;
    using Mono.Data.Sqlite;
    using System;
    using System.Data;
    using System.Globalization;

    public class SqliteTableLoader
    {
        public ITable table;

        private SqliteCommand stage;

        private SqliteParameter[] _p;

        private Func<string, object>[] _conv;

        public SqliteTableLoader(ITable table)
        {
            this.table = table;
        }

        public void StartStaging(SqliteConnection stagingConnection, SqliteTransaction transaction, string[] fieldNames)
        {
            this.stage = SqliteHelper.GetStageCommand(stagingConnection, table);
            this.stage.Transaction = transaction;
            var n = stage.Parameters.Count;
            _p = new SqliteParameter[n];
            _conv = new Func<string, object>[n];
            for (int i = 0; i < _p.Length; i++)
            {
                _p[i] = stage.Parameters[i];
                _conv[i] = _p[i].DbType switch
                {
                    DbType.String => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : s,
                    DbType.Single or DbType.Double => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : float.Parse(s, CultureInfo.InvariantCulture),
                    DbType.Int64 or DbType.Int32 or DbType.Int16 => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : int.Parse(s, CultureInfo.InvariantCulture),
                    _ => (s) => string.IsNullOrEmpty(s) ? DBNull.Value : s
                };
            }
        }

        public void StageLine(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                _p[i].Value = _conv[i](line[i]);
            }
            stage.ExecuteNonQuery();
        }

        public void EndStaging()
        {
            stage.Dispose();
        }

        public void MergeStaging(SqliteConnection mergeConnexion)
        {
            SqliteHelper.MergeStagingTable(mergeConnexion, table);
        }
    }
}
