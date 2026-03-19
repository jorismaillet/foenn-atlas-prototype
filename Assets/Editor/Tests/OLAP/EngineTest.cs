using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;
using NUnit.Framework;

namespace Assets.Editor.Tests.OLAP
{
    public class EngineTest
    {
        [Test]
        public void TestQueryExecuteAndResult()
        {
            Env.DatabasePath = SqliteHelper.DATABASE_TEST_PATH;
            using (var connection = SqliteHelper.CreateConnection())
            {
                var dataset = WeatherHistoryDataset.Instance;
                dataset.InitTables(connection);
                SqliteHelper.Insert(connection, dataset.time,
                    new List<Field> { dataset.time.hour },
                    new List<string> { "18" });
                SqliteHelper.Insert(connection, dataset.location,
                    new List<Field> { dataset.location.Department, dataset.location.PostName },
                    new List<string> { "29", "Station météo Plomelin" });
                SqliteHelper.Insert(connection, dataset.location,
                    new List<Field> { dataset.location.Department, dataset.location.PostName },
                    new List<string> { "29", "Station météo Brest" });
                SqliteHelper.Insert(connection, dataset.coreFact,
                    new List<Field> { dataset.coreFact.temperature, dataset.coreFact.locationRef, dataset.coreFact.timeRef },
                    new List<string> { "20", "1", "1" });
                SqliteHelper.Insert(connection, dataset.coreFact,
                    new List<Field> { dataset.coreFact.temperature, dataset.coreFact.locationRef, dataset.coreFact.timeRef },
                    new List<string> { "21", "2", "1" });

                var queryRequest = new QueryRequest(dataset.coreFact)
                    .SelectAvg(dataset.coreFact.temperature)
                    .SelectGroup(dataset.location.PostName, dataset.time.hour)
                    .WhereEq(dataset.location.Department, "29")
                    .Join(dataset.coreFact.locationRef)
                    .Join(dataset.coreFact.timeRef)
                    .GroupBy(dataset.location.PostName);

                var queryResult = queryRequest.Execute(connection);

                Assert.AreEqual(queryResult.rows.Count, 2);

                Assert.AreEqual(queryResult.rows[0].values.Count, 3);
                Assert.AreEqual(queryResult.rows[0].values[dataset.location.PostName], "Station météo Brest");
                Assert.AreEqual(queryResult.rows[0].values[dataset.coreFact.temperature], 21F);
                Assert.AreEqual(queryResult.rows[0].values[dataset.time.hour], 18);

                Assert.AreEqual(queryResult.rows[1].values.Count, 3);
                Assert.AreEqual(queryResult.rows[1].StringValue(dataset.location.PostName), "Station météo Plomelin");
                Assert.AreEqual(queryResult.rows[1].FloatValue(dataset.coreFact.temperature), 20F);
                Assert.AreEqual(queryResult.rows[1].IntValue(dataset.time.hour), 18);
            }
        }
    }
}
