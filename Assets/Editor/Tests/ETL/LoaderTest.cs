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
    public class LoaderTest
    {
        [Test]
        public void TestInsert()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
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
