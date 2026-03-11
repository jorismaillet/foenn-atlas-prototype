namespace Assets.Editor.Tests.Atlas.Models
{
    using Assets.Scripts.Foenn.Atlas.Models.Activities;
    using Assets.Scripts.Foenn.Atlas.Models.Condition;
    using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using NUnit.Framework;

    public class ActivityTest
    {
        [Test]
        public void TestActivityCreation()
        {
            var hourCondition = new HourRangeCondition(14, 16);
            var temp = new MetricGroup("Temperature", WeatherFact.temperature, WeatherFact.temperature_10, WeatherFact.temperature_20);
            var tempCondition = new GroupAnyCondition(temp, 25, 30);
            var rainCondition = new MetricRangeCondition(WeatherFact.rain_1, 0, 0);
            var activity = new Activity("Beach", hourCondition, tempCondition, rainCondition);
            var row = new Row() { time = TimeField.AAAAMMJJHH("2023091515") };
            row.values.Add(WeatherFact.rain_1, new Measure(WeatherFact.rain_1, 0));
            row.values.Add(WeatherFact.temperature, new Measure(WeatherFact.temperature, 25));
            Assert.IsTrue(activity.conditions.IsMatch(row));
        }
    }
}
