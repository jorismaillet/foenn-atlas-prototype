using Assets.Scripts;
using Assets.Scripts.Database;
using Assets.Scripts.ETL;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using Mono.Data.Sqlite;
using NUnit.Framework;

namespace Assets.Editor.Tests.ETL
{
    [TestFixture]
    public class ETLProcessorTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            DatabaseHelper.CreateDb();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            using (var connection = SqliteHelper.CreateConnection())
            {
                foreach (var table in WeatherHistoryDataset.Tables)
                {
                    SqliteHelper.DropStagingTable(connection, table);
                    SqliteHelper.Execute(connection, $"DROP TABLE IF EXISTS {table.name}");
                    SqliteHelper.Execute(connection, $"DROP TABLE IF EXISTS {MetadataTable.MakeTableName(table.name)}");
                }
            }
        }

        [Test]
        public void TestETLProcessor()
        {
            using (var connection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(connection);
                var fileName = "Tests/Weathers/H_29_latest-2023-2024.csv";
                var processor = new ETLProcessor(fileName, WeatherHistoryDataset.Dimensions, WeatherHistoryDataset.Facts);
                processor.ProcessETL();

                var res = new QueryRequest(WeatherHistoryDataset.fact).Execute(connection);
                Assert.AreEqual(res.rows.Count, 4);
            }
        }
    }
}
