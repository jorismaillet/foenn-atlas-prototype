using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Engine.Sql.Filters;
using Assets.Scripts.OLAP.Engine.Sql.Joins;
using Assets.Scripts.OLAP.Schema;
using NUnit.Framework;

namespace Assets.Editor.Tests.OLAP
{
    public class EngineTest
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
        public void TestToSql_SelectAllFromFactByDefault()
        {
            var queryRequest = new QueryRequest(WeatherHistoryDataset.fact);

            var sql = queryRequest.ToSql();

            Assert.AreEqual("SELECT * FROM \"weather_history_facts\"", sql);
        }
        [Test]
        public void TestToSql_SelectOneFromFactByDefault()
        {
            var queryRequest = new QueryRequest(WeatherHistoryDataset.fact).Select(WeatherHistoryDataset.fact.PrimaryKey);

            var sql = queryRequest.ToSql();

            Assert.AreEqual("SELECT \"ID\" FROM \"weather_history_facts\"", sql);
        }

        [Test]
        public void TestToSql_WithWhereAndGroupBy_GeneratesExpectedClauses()
        {
            var loc = WeatherHistoryDataset.location;
            var time = WeatherHistoryDataset.time;

            var queryRequest = new QueryRequest(WeatherHistoryDataset.fact)
                .Where(new DataFilter(LocationDimension.Department.Of(loc), DataFilterMode.INCLUDE, "29"))
                .Where(new DataFilter(TimeDimension.day.Of(time), DataFilterMode.INCLUDE, 15))
                .GroupBy(LocationDimension.PostName.Of(loc));

            var sql = queryRequest.ToSql();

            StringAssert.StartsWith("SELECT * FROM \"weather_history_facts\"", sql);
            StringAssert.Contains(" WHERE \"location_dimension\".\"department\"=\"29\" AND \"time_dimension\".\"day\"=15", sql);
            StringAssert.Contains(" GROUP BY \"location_dimension\".\"post_name\"", sql);
        }

        [Test]
        public void TestToSql_WithJoin_GeneratesJoinClause()
        {
            var fact = WeatherHistoryDataset.fact;

            var queryRequest = new QueryRequest(fact)
                .Join(fact, fact.locationRef, JoinType.INNER);

            var sql = queryRequest.ToSql();

            StringAssert.Contains("INNER JOIN location_dimension ON \"weather_history_facts\".\"location_id\" = \"location_dimension\".\"ID\"", sql);
        }

        [Test]
        public void TestQueryExecuteAndResult()
        {
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
                SqliteHelper.Insert(connection, WeatherHistoryDataset.fact.name,
                    new List<Field> { WeatherFact.temperature, WeatherHistoryDataset.fact.locationRef, WeatherHistoryDataset.fact.timeRef },
                    new List<string> { "20", "1", "1" });
                SqliteHelper.Insert(connection, WeatherHistoryDataset.fact.name,
                    new List<Field> { WeatherFact.temperature, WeatherHistoryDataset.fact.locationRef, WeatherHistoryDataset.fact.timeRef },
                    new List<string> { "21", "2", "1" });

                var queryRequest = new QueryRequest(WeatherHistoryDataset.fact)
                    .Select(AggregationKey.AVG, WeatherFact.temperature)
                    .Select(LocationDimension.Department, TimeDimension.timestamp)
                    .Where(new DataFilter(LocationDimension.Department, DataFilterMode.INCLUDE, "29"))
                    .Join(WeatherHistoryDataset.fact, WeatherHistoryDataset.fact.locationRef, JoinType.INNER)
                    .Join(WeatherHistoryDataset.fact, WeatherHistoryDataset.fact.timeRef, JoinType.INNER)
                    .GroupBy(LocationDimension.PostName);

                var queryResult = queryRequest.Execute(connection);

                Assert.AreEqual(queryResult.rows.Count, 2);

                Assert.AreEqual(queryResult.rows[0].values.Count, 3);
                Assert.AreEqual(queryResult.rows[0].values[LocationDimension.PostName], "Station météo Plomelin");
                Assert.AreEqual(queryResult.rows[0].values[WeatherFact.temperature], "20");
                Assert.AreEqual(((DateTime)queryResult.rows[0].values[TimeDimension.timestamp]).Hour, 18);

                Assert.AreEqual(queryResult.rows[1].values.Count, 3);
                Assert.AreEqual(queryResult.rows[1].values[LocationDimension.PostName], "Station météo Plomelin");
                Assert.AreEqual(queryResult.rows[1].values[WeatherFact.temperature], "21");
                Assert.AreEqual(((DateTime)queryResult.rows[1].values[TimeDimension.timestamp]).Hour, 18);
            }
        }
    }
}
