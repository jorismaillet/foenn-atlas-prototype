using Assets.Scripts.Foenn.Datasets.Facts;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Datasets;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Dimensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Editor.Tests.Engine
{
    public class QueryResultTest
    {
        [Test]
        public void TestQueryResultCreation()
        {
            var queryRequest = new QueryRequest(WeatherHistoryDataset.fact)
                .Select(AggregationKey.AVG, WeatherHistoryDataset.fact, WeatherFact.temperature)
                .Select(WeatherHistoryDataset.location, LocationDimension.Department)
                .Select(WeatherHistoryDataset.time, TimeDimension.yyyymmddhh)
                .Where(new DataFilter(new PrefixedField(WeatherHistoryDataset.location, LocationDimension.Department), DataFilterMode.INCLUDE, "29"))
                .GroupBy(WeatherHistoryDataset.location, LocationDimension.PostName);
            var headers = new string[] { "T", "DPT", "AAAAMMJJHH", "NUM_POSTE" };
            List<string[]> rawResult = new List<string[]>();
            rawResult.Add(new string[] { "20", "29", "2025013018", "Station météo Plomelin"});
            rawResult.Add(new string[] { "20", "29", "2025013018", "Station météo Brest" });
            var queryResult = new QueryResult(headers);
            rawResult.ForEach(line => queryResult.ParseLine(line));
            Assert.AreEqual(queryResult.rows.Count, 2);
            Assert.AreEqual(queryResult.rows[0].values.Count, 1);
            Assert.AreEqual(queryResult.rows[0].values[LocationDimension.PostName], "Station météo Plomelin");
            Assert.AreEqual(queryResult.rows[0].time.start.Hour, 18);
            Assert.AreEqual(queryResult.rows[0].values[LocationDimension.Department],"29");
            Assert.AreEqual(queryResult.rows[0].values.Count, 1);
            Assert.AreEqual(queryResult.rows[0].values[WeatherFact.temperature_10], 20);
            Assert.AreEqual(queryResult.rows[1].values[LocationDimension.PostName], "Station météo Brest");
            Assert.AreEqual(queryResult.rows[1].values.Count, 1);
            Assert.AreEqual(queryResult.rows[1].time.start.Hour, 18);
            Assert.AreEqual(queryResult.rows[1].values[LocationDimension.Department], "29");
            Assert.AreEqual(queryResult.rows[1].values.Count, 1);
            Assert.AreEqual(queryResult.rows[1].values[WeatherFact.temperature_10], 20);
        }
    }
}