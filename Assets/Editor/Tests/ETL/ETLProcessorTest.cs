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
                var processor = new ETLProcessor(fileName, WeatherHistoryDataset.Dimensions, WeatherHistoryDataset.Facts);
                processor.ProcessETL(connection);

                var res = new QueryRequest(WeatherHistoryDataset.fact).Execute(connection);
                Assert.AreEqual(res.rows.Count, 4);
            }
        }
    }
}
