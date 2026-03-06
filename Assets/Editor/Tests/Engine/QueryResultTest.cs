using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Editor.Tests.Engine
{
    public class QueryResultTest
    {
        [Test]
        public void TestQueryResultCreation()
        {
            var queryRequest = new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Metric(WeatherHistoryMetricKey.T, AggregationKey.AVG))
                .Select(new Attribute(WeatherHistoryAttributeKey.DPT))
                .Select(new Attribute(WeatherHistoryAttributeKey.AAAAMMJJHH))
                .Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.DPT, "29"))
                .GroupBy(WeatherHistoryAttributeKey.NUM_POSTE);
            var headers = new string[] { "T", "DPT", "AAAAMMJJHH", "NUM_POSTE" };
            List<string[]> rawResult = new List<string[]>();
            rawResult.Add(new string[] { "20", "29", "2025013018", "Station météo Plomelin"});
            rawResult.Add(new string[] { "20", "29", "2025013018", "Station météo Brest" });
            var queryResult = new QueryResult(headers);
            rawResult.ForEach(line => queryResult.ParseLine(line));
            Assert.AreEqual(queryResult.rows.Count, 2);
            Assert.AreEqual(queryResult.rows[0].attributes[WeatherHistoryAttributeKey.NUM_POSTE].value, "Station météo Plomelin");
            Assert.AreEqual(queryResult.rows[0].attributes.Count, 1);
            Assert.AreEqual(queryResult.rows[0].time.start.Hour, 18);
            Assert.AreEqual(queryResult.rows[0].attributes[0].attribute.key, WeatherHistoryAttributeKey.DPT);
            Assert.AreEqual(queryResult.rows[0].attributes[0].value, "29");
            Assert.AreEqual(queryResult.rows[0].measures.Count, 1);
            Assert.AreEqual(queryResult.rows[0].measures[0].metric.key, WeatherHistoryMetricKey.T);
            Assert.AreEqual(queryResult.rows[0].measures[0].value, 20);
            Assert.AreEqual(queryResult.rows[1].attributes[WeatherHistoryAttributeKey.NUM_POSTE].value, "Station météo Brest");
            Assert.AreEqual(queryResult.rows[1].attributes.Count, 1);
            Assert.AreEqual(queryResult.rows[1].time.start.Hour, 18);
            Assert.AreEqual(queryResult.rows[1].attributes[0].attribute.key, WeatherHistoryAttributeKey.DPT);
            Assert.AreEqual(queryResult.rows[1].attributes[0].value, "29");
            Assert.AreEqual(queryResult.rows[1].measures.Count, 1);
            Assert.AreEqual(queryResult.rows[1].measures[0].metric.key, WeatherHistoryMetricKey.T);
            Assert.AreEqual(queryResult.rows[1].measures[0].value, 20);
        }
    }
}