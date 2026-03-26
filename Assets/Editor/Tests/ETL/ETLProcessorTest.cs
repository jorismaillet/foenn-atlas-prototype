using Assets.Scripts;
using Assets.Scripts.ETL;
using Assets.Scripts.Helpers;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using NUnit.Framework;

namespace Assets.Editor.Tests.ETL
{
    public class ETLProcessorTest
    {
        [Test]
        public void TestETLProcessor()
        {
            Env.DatabasePath = SqliteHelper.DATABASE_TEST_PATH;
            using (var connection = SqliteHelper.CreateConnection())
            {
                var dataset = WeatherHistoryDataset.Instance;
                dataset.InitTables(connection);
                var fileName = "Tests/Weathers/H_29_latest-2023-2024.csv";
                var processor = new ETLProcessor(fileName, dataset);
                processor.ProcessETL(connection);

                var res = new QueryRequest(dataset.coreFact).Execute(connection);
                Assert.AreEqual(res.rows.Count, 4);
            }
        }
    }
}
