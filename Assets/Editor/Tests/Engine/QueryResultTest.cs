namespace Assets.Editor.Tests.Engine
{
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Assets.Scripts.Foenn.OLAP.Sql;
    using NUnit.Framework;
    using System.Collections.Generic;

    public class QueryResultTest
    {
        [Test]
        public void TestQueryResultCreation()
        {
            var loc = WeatherHistoryDataset.location;
            var time = WeatherHistoryDataset.time;

            var queryRequest = new QueryRequest(WeatherHistoryDataset.fact)
                .Select(AggregationKey.AVG, WeatherFact.temperature)
                .Select(loc, LocationDimension.Department)
                .Select(time, TimeDimension.yyyymmddhh)
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
            Assert.AreEqual(queryResult.rows[0].time.start.Hour, 18);
            Assert.AreEqual(queryResult.rows[0].values[LocationDimension.Department], "29");
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
