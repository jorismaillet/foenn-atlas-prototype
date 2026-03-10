using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Condition;
using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using NUnit.Framework;

namespace Assets.Editor.Tests.Atlas.Models
{
    public class ActivityTest
    {
        [Test]
        public void TestActivityCreation()
        {
            var hourCondition = new HourRangeCondition(14, 16);
            var temp = new MetricGroup("Temperature", WeatherHistoryMetricKey.T, WeatherHistoryMetricKey.T10, WeatherHistoryMetricKey.T20);
            var tempCondition = new GroupAnyCondition(temp, 25, 30);
            var rainCondition = new MetricRangeCondition(WeatherHistoryMetricKey.RR1, 0, 0);
            var activity = new Activity("Beach", hourCondition, tempCondition, rainCondition);
            var row = new Row() { time = TimeField.AAAAMMJJHH("2023091515") };
            row.measures.Add(WeatherHistoryMetricKey.RR1, new Measure(new Metric(WeatherHistoryMetricKey.RR1, AggregationKey.MAX), 0));
            row.measures.Add(WeatherHistoryMetricKey.T, new Measure(new Metric(WeatherHistoryMetricKey.T, AggregationKey.AVG), 25));
            Assert.IsTrue(activity.conditions.IsMatch(row));
        }
    }
}