using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema;
using NUnit.Framework;

namespace Assets.Editor.Tests.OLAP
{
    public class EngineTest
    {
        [Test]
        public void TestQueryExecuteAndResult()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            using (var connection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(connection);
                SqliteHelper.Insert(connection, WeatherHistoryDataset.time.name,
                    new List<Field> { TimeDimension.hour },
                    new List<string> { "18" });
                SqliteHelper.Insert(connection, WeatherHistoryDataset.location.name,
                    new List<Field> { LocationDimension.Department, LocationDimension.PostName },
                    new List<string> { "29", "Station météo Plomelin" });
                SqliteHelper.Insert(connection, WeatherHistoryDataset.location.name,
                    new List<Field> { LocationDimension.Department, LocationDimension.PostName },
                    new List<string> { "29", "Station météo Brest" });
                SqliteHelper.Insert(connection, WeatherHistoryDataset.coreFact.name,
                    new List<Field> { WeatherCoreFact.temperature, WeatherHistoryDataset.coreFact.locationRef, WeatherHistoryDataset.coreFact.timeRef },
                    new List<string> { "20", "1", "1" });
                SqliteHelper.Insert(connection, WeatherHistoryDataset.coreFact.name,
                    new List<Field> { WeatherCoreFact.temperature, WeatherHistoryDataset.coreFact.locationRef, WeatherHistoryDataset.coreFact.timeRef },
                    new List<string> { "21", "2", "1" });

                var queryRequest = new QueryRequest(WeatherHistoryDataset.coreFact)
                    .SelectAvg(WeatherCoreFact.temperature)
                    .Select(LocationDimension.PostName, TimeDimension.hour)
                    .WhereEq(LocationDimension.Department, "29")
                    .Join(WeatherHistoryDataset.coreFact.locationRef)
                    .Join(WeatherHistoryDataset.coreFact.timeRef)
                    .GroupBy(LocationDimension.PostName);

                var queryResult = queryRequest.Execute(connection);

                Assert.AreEqual(queryResult.rows.Count, 2);

                Assert.AreEqual(queryResult.rows[0].values.Count, 3);
                Assert.AreEqual(queryResult.rows[0].values[LocationDimension.PostName], "Station météo Brest");
                Assert.AreEqual(queryResult.rows[0].values[WeatherCoreFact.temperature], 21F);
                Assert.AreEqual(queryResult.rows[0].values[TimeDimension.hour], 18);

                Assert.AreEqual(queryResult.rows[1].values.Count, 3);
                Assert.AreEqual(queryResult.rows[1].StringValue(LocationDimension.PostName), "Station météo Plomelin");
                Assert.AreEqual(queryResult.rows[1].FloatValue(WeatherCoreFact.temperature), 20F);
                Assert.AreEqual(queryResult.rows[1].IntValue(TimeDimension.hour), 18);
            }
        }
    }
}
