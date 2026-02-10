using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.CSV;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using NUnit.Framework;

namespace Assets.Editor.Tests.Atlas.Engine
{
    public class QueryRequestTest
    {
        [Test]
        public void ToSqlTest() {
            var queryRequest = new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Metric(WeatherHistoryMetricKey.T, AggregationKey.AVG))
                .Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.DPT, "29"))
                .GroupBy(WeatherHistoryAttributeKey.NUM_POSTE);
            var sqlExpect = "SELECT AVG(\"T\") FROM \"weather_data\" WHERE \"DPT\"=\"29\" GROUP BY \"NUM_POSTE\";";
            Assert.AreEqual(queryRequest.ToSql(new SqliteDialect()), sqlExpect);
        }
    }
}