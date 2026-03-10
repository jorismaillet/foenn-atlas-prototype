namespace Assets.Editor.Tests.ETL
{
    using Assets.Scripts.Foenn;
    using Assets.Scripts.Foenn.Engine.Connectors;
    using Assets.Scripts.Foenn.Engine.Execution;
    using Assets.Scripts.Foenn.ETL.Datasets;
    using Assets.Scripts.Foenn.ETL.Datasources;
    using Assets.Scripts.Foenn.ETL.Loaders;
    using Mono.Data.Sqlite;
    using NUnit.Framework;

    public class LoaderTest
    {
        [Test]
        public void TestInsert()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            using (var connection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(connection);
                var loader = new SqliteTableLoader(WeatherHistoryDataset.fact);
                SqliteHelper.Execute(connection, $"INSERT INTO \"{WeatherHistoryDataset.fact.Name}\" (ID) VALUES (1);");
                var res = new QueryRequest(WeatherHistoryDataset.fact).Execute(connection);
                Assert.AreEqual(res.rows.Count, 1);
                CleanupDataset(connection);
            }
        }

        [Test]
        public void TestLoad()
        {
        }

        private void CleanupDataset(SqliteConnection connection)
        {
            foreach (var table in WeatherHistoryDataset.Tables)
            {
                SqliteHelper.DropStagingTable(connection, table);
                SqliteHelper.Execute(connection, $"DROP TABLE IF EXISTS {table.Name}");
                SqliteHelper.Execute(connection, $"DROP TABLE IF EXISTS {MetadataTable.TableName(table.Name)}");
            }
        }
    }
}
