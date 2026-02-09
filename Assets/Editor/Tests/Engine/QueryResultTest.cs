using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.ETL.CSV;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Editor.Tests.Atlas.Engine
{
    public class QueryResultTest
    {
        [Test]
        public void TestQueryResultCreation()
        {
            var queryRequest = new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Metric(MetricKey.T, AggregationKey.AVG))
                .Where(new DataFilter(DataFilterMode.INCLUDE, AttributeKey.DPT, "29"))
                .GroupBy(AttributeKey.NUM_POSTE);
            List<string> headers = new List<string>() { "AAAAMMJJHH", "DPT", "NUM_POSTE", "T" };
            List<List<string>> rawResult = new List<List<string>>();
            rawResult.Add(new List<string>() { "2025013018", "29", "Station météo Plomelin", "20" });
            rawResult.Add(new List<string>() { "2025013018", "29", "Station météo Brest", "20" });
            var queryResult = new QueryResult(queryRequest, headers, rawResult);
            Assert.AreEqual(headers, queryResult.rawHeaders);
            Assert.AreEqual(queryResult.rows.Count, 2);
            Assert.AreEqual(queryResult.rows[0].geo.numPost, "Station météo Plomelin");
            Assert.AreEqual(queryResult.rows[0].attributes.Count, 1);
            Assert.AreEqual(queryResult.rows[0].time.start.Hour, 18);
            Assert.AreEqual(queryResult.rows[0].attributes[0].key, AttributeKey.DPT);
            Assert.AreEqual(queryResult.rows[0].attributes[0].value, "29");
            Assert.AreEqual(queryResult.rows[0].measures.Count, 1);
            Assert.AreEqual(queryResult.rows[0].measures[0].metric.key, MetricKey.T);
            Assert.AreEqual(queryResult.rows[0].measures[0].value, 20);
            Assert.AreEqual(queryResult.rows[1].geo.numPost, "Station météo Brest");
            Assert.AreEqual(queryResult.rows[1].attributes.Count, 1);
            Assert.AreEqual(queryResult.rows[1].time.start.Hour, 18);
            Assert.AreEqual(queryResult.rows[1].attributes[0].key, AttributeKey.DPT);
            Assert.AreEqual(queryResult.rows[1].attributes[0].value, "29");
            Assert.AreEqual(queryResult.rows[1].measures.Count, 1);
            Assert.AreEqual(queryResult.rows[1].measures[0].metric.key, MetricKey.T);
            Assert.AreEqual(queryResult.rows[1].measures[0].value, 20);
        }
    }
}