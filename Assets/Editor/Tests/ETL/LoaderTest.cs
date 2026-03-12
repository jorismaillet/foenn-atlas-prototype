using Assets.Scripts;
using Assets.Scripts.Database;
using Assets.Scripts.ETL.Loaders;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using Mono.Data.Sqlite;
using NUnit.Framework;

namespace Assets.Editor.Tests.ETL
{
    [TestFixture]
    public class LoaderTest
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
        public void TestInsert()
        {
            using (var connection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(connection);
                SqliteHelper.Execute(connection, $"INSERT INTO \"{WeatherHistoryDataset.fact.name}\" (temperature) VALUES (20);");
                var res = new QueryRequest(WeatherHistoryDataset.fact).Execute(connection);
                Assert.AreEqual(res.rows.Count, 1);
            }
        }
    }
}
