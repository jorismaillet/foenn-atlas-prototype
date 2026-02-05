using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Condition;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.OLAP;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Times;
using NUnit.Framework;

public class ActivityTest
{
    [Test]
    public void TestActivityCreation()
    {
        var hourCondition = new HourRangeCondition(14, 16);
        var temp = new MetricGroup("Temperature", MetricKey.T, MetricKey.T10, MetricKey.T20);
        var tempCondition = new GroupAnyCondition(temp, 25, 30);
        var rainCondition = new MetricRangeCondition(MetricKey.RR1, 0, 0);
        var activity = new Activity("Beach", hourCondition, tempCondition, rainCondition);
        var row = new Row() { time = TimeDimension.AAAAMMJJHH("2023091515") };
        row.measures.Add(new Measure(new Metric(MetricKey.RR1, AggregationKey.MAX), 0));
        row.measures.Add(new Measure(new Metric(MetricKey.T, AggregationKey.AVG), 25));
        Assert.IsTrue(activity.conditions.IsMatch(row));
    }
}
