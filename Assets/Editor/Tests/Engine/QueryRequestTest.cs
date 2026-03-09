using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using NUnit.Framework;

namespace Assets.Editor.Tests.Engine
{
    public class QueryRequestTest
    {
        [Test]
        public void ToSqlTest() {
            //var queryRequest = new QueryRequest(WeatherHistoryDatasource.tableName)
            //    .Select(new Metric(WeatherHistoryMetricKey.T, AggregationKey.AVG))
            //    .Select(new Attribute(WeatherHistoryAttributeKey.NUM_POSTE))
            //    .Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.DPT, "29"))
            //    .GroupBy(WeatherHistoryAttributeKey.NUM_POSTE);
            //var sqlExpect = "SELECT AVG(\"T\"), \"NUM_POSTE\" FROM \"weather_data\" WHERE \"DPT\"=\"29\" GROUP BY \"NUM_POSTE\";";
            //Assert.AreEqual(queryRequest.ToSql(new SqliteDialect()), sqlExpect);
        }
    }
}