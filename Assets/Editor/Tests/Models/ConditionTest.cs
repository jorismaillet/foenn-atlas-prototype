using Assets.Scripts.Helpers;
using Assets.Scripts.Models.Condition;
using Assets.Scripts.Models.Condition.Definitions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using NUnit.Framework;

namespace Assets.Editor.Tests.Models
{
    public class CustomCondition : ICondition
    {
        private bool overrideCondition;

        public CustomCondition(bool overrideCondition)
        {
            this.overrideCondition = overrideCondition;
        }

        public bool IsMatch(Row row)
        {
            return overrideCondition;
        }

        public QueryRequest AddToQuery(QueryRequest query)
        {
            return query;
        }
    }

    public class ConditionTest
    {
        //HourRangeCondition
        [Test]
        public void TestInsideHourRangeCondition()
        {
            var time = WeatherHistoryDataset.Instance.time;
            var c = new IntRangeCondition(time.hour, 8, 10);
            var row = new Row();
            row.values[time.hour] = 8;
            Assert.IsTrue(c.IsMatch(row));
        }

        [Test]
        public void TestEdgeHourRangeCondition()
        {
            var time = WeatherHistoryDataset.Instance.time;
            var c = new IntRangeCondition(time.hour, 8, 10);
            var row = new Row();
            row.values[time.hour] = 11;
            Assert.IsFalse(c.IsMatch(row));
        }

        //MetricRangeCondition
        [Test]
        public void TestInsideMetricRangeCondition()
        {
            var time = WeatherHistoryDataset.Instance.time;
            var c = new FloatRangeCondition(time.hour, 20, 25);
            var row = new Row();
            row.values[time.hour] = 20;
            Assert.IsTrue(c.IsMatch(row));
        }

        [Test]
        public void TestOutsideMetricRangeCondition()
        {
            var time = WeatherHistoryDataset.Instance.time;
            var c = new FloatRangeCondition(time.hour, 20, 25);
            var row = new Row();
            row.values[time.hour] = 30;
            Assert.IsFalse(c.IsMatch(row));
        }

        [Test]
        public void TestEdgeTimeRangeCondition()
        {
            var time = WeatherHistoryDataset.Instance.time;
            var c = new TimeRangeCondition(time, DateTimeHelper.ToDateTime("2022080110"), DateTimeHelper.ToDateTime("2022080112"));
            var row = new Row();
            row.values[time.timestamp] = DateTimeHelper.ToTimestamp("2022080112");
            row.values[time.duration] = 1;
            Assert.IsFalse(c.IsMatch(row));
        }

        [Test]
        public void TestInsideTimeRangeCondition()
        {
            var time = WeatherHistoryDataset.Instance.time;
            var c = new TimeRangeCondition(time, DateTimeHelper.ToDateTime("2022080110"), DateTimeHelper.ToDateTime("2022080112"));
            var row = new Row();
            row.values[time.timestamp] = DateTimeHelper.ToTimestamp("2022080110");
            row.values[time.duration] = 1;
            Assert.IsTrue(c.IsMatch(row));
        }

        //AllCondition
        [Test]
        public void TestAllTrueAllCondition()
        {
            var c = new AllCondition(new CustomCondition(true), new CustomCondition(true));
            Assert.IsTrue(c.IsMatch(new Row()));
        }

        [Test]
        public void TestOneTrueAllCondition()
        {
            var c = new AllCondition(new CustomCondition(true), new CustomCondition(false));
            Assert.IsFalse(c.IsMatch(new Row()));
        }

        [Test]
        public void TestNoneTrueAllCondition()
        {
            var c = new AllCondition(new CustomCondition(false), new CustomCondition(false));
            Assert.IsFalse(c.IsMatch(new Row()));
        }

        [Test]
        public void TestEmptyAllCondition()
        {
            var c = new AllCondition();
            Assert.IsTrue(c.IsMatch(new Row()));
        }

        //AnyCondition
        [Test]
        public void TestAllTrueAnyCondition()
        {
            var c = new AnyCondition(new CustomCondition(true), new CustomCondition(true));
            Assert.IsTrue(c.IsMatch(new Row()));
        }

        [Test]
        public void TestOneTrueAnyCondition()
        {
            var c = new AnyCondition(new CustomCondition(true), new CustomCondition(false));
            Assert.IsTrue(c.IsMatch(new Row()));
        }

        [Test]
        public void TestNoneTrueAnyCondition()
        {
            var c = new AnyCondition(new CustomCondition(false), new CustomCondition(false));
            Assert.IsFalse(c.IsMatch(new Row()));
        }

        [Test]
        public void TestEmptyAnyCondition()
        {
            var c = new AnyCondition();
            Assert.IsFalse(c.IsMatch(new Row()));
        }
    }
}
