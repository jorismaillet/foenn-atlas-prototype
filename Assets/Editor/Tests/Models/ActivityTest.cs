using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Condition;
using Assets.Scripts.Models.Condition.Definitions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts;
using Assets.Scripts.OLAP.Engine.Result;
using NUnit.Framework;

namespace Assets.Editor.Tests.Models
{
    public class ActivityTest
    {
        [Test]
        public void TestActivityCreation()
        {
            var hourCondition = new HourRangeCondition(14, 16);
            var temp = new MetricGroup("Temperature", WeatherFact.temperature, WeatherExtraFact.temperature10, WeatherExtraFact.temperature20);
            var tempCondition = new GroupAnyCondition(temp, 25, 30);
            var rainCondition = new MetricRangeCondition(WeatherFact.rain, 0, 0);
            var activity = new Activity("Beach", hourCondition, tempCondition, rainCondition);
            var row = new Row();
            row.values[TimeDimension.timestamp] = TimeDimension.ToTimestamp("2023091515");
            row.values[WeatherFact.rain] = new Measure(WeatherFact.rain, 0);
            row.values[WeatherFact.temperature] = new Measure(WeatherFact.temperature, 25);
            Assert.IsTrue(activity.conditions.IsMatch(row));
        }
    }
}
