namespace Assets.Editor.Tests.ETL
{
    using Assets.Scripts.Foenn;
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.ETL;
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Assets.Scripts.Foenn.OLAP.Query;
    using Mono.Data.Sqlite;
    using NUnit.Framework;

    public class ETLProcessorTest
    {
        [Test]
        public void TestETLProcessor()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            using (var connection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(connection);
                var fileName = "Tests/Weathers/H_29_latest-2023-2024.csv";
                var processor = new ETLProcessor(fileName);
                processor.ProcessETL();

                var res = new QueryRequest(WeatherHistoryDataset.fact).Execute(connection);
                Assert.AreEqual(res.rows.Count, 4);

                CleanupDataset(connection);
            }
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
