using System;
using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Engine.Sql.Filters;
using Assets.Scripts.OLAP.Schema;
using NUnit.Framework;

namespace Assets.Editor.Tests.OLAP
{
    public class EngineTest
    {
        [Test]
        public void TestQueryResultCreation()
        {
            var loc = WeatherHistoryDataset.location;
            var time = WeatherHistoryDataset.time;

            var queryRequest = new QueryRequest(WeatherHistoryDataset.fact)
                .Select(AggregationKey.AVG, WeatherFact.temperature)
                .Select(loc, LocationDimension.Department)
                .Select(time, TimeDimension.timestamp)
                .Where(new DataFilter(LocationDimension.Department.Of(loc), DataFilterMode.INCLUDE, "29"))
                .GroupBy(LocationDimension.PostName);
            var headers = new string[] { "T", "DPT", "AAAAMMJJHH", "NUM_POSTE" };
            List<string[]> rawResult = new List<string[]>();
            rawResult.Add(new string[] { "20", "29", "2025013018", "Station météo Plomelin" });
            rawResult.Add(new string[] { "20", "29", "2025013018", "Station météo Brest" });
            var queryResult = new QueryResult(headers, queryRequest.selectedColumns);
            rawResult.ForEach(line => queryResult.ParseLine(line));
            Assert.AreEqual(queryResult.rows.Count, 2);
            Assert.AreEqual(queryResult.rows[0].values.Count, 1);
            Assert.AreEqual(queryResult.rows[0].values[LocationDimension.PostName], "Station météo Plomelin");
            Assert.AreEqual(((DateTime)queryResult.rows[0].values[TimeDimension.timestamp]).Hour, 18);
            Assert.AreEqual(queryResult.rows[0].values[LocationDimension.Department], "29");
            Assert.AreEqual(queryResult.rows[0].values.Count, 1);
            Assert.AreEqual(queryResult.rows[0].values[WeatherExtraFact.temperature10], 20);
            Assert.AreEqual(queryResult.rows[1].values[LocationDimension.PostName], "Station météo Brest");
            Assert.AreEqual(queryResult.rows[1].values.Count, 1);
            Assert.AreEqual(((DateTime)queryResult.rows[1].values[TimeDimension.timestamp]).Hour, 18);
            Assert.AreEqual(queryResult.rows[1].values[LocationDimension.Department], "29");
            Assert.AreEqual(queryResult.rows[1].values.Count, 1);
            Assert.AreEqual(queryResult.rows[1].values[WeatherExtraFact.temperature10], 20);
        }
    }
}
