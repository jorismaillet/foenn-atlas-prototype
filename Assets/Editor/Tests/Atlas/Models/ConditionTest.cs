using Assets.Scripts.Foenn.Atlas.Models.Condition;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.OLAP;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Times;
using NUnit.Framework;

namespace Assets.Editor.Tests.Atlas.Models
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
    }

    public class ConditionTest
    {
        //HourRangeCondition
        [Test]
        public void TestInsideHourRangeCondition()
        {
            var c = new HourRangeCondition(8, 10);
            var row = new Row() { time = TimeDimension.AAAAMMJJHH("2023091508") };
            Assert.IsTrue(c.IsMatch(row));
        }
        [Test]
        public void TestOutsideHourRangeCondition()
        {
            var c = new HourRangeCondition(8, 10);
            var row = new Row() { time = TimeDimension.AAAAMMJJHH("2023091510") };
            Assert.IsFalse(c.IsMatch(row));
        }

        //MetricRangeCondition
        [Test]
        public void TestInsideMetricRangeCondition()
        {
            var c = new MetricRangeCondition(MetricKey.T, 20, 25);
            var row = new Row();
            row.measures.Add(new Measure(new Metric(MetricKey.T, AggregationKey.AVG), 20));
            Assert.IsTrue(c.IsMatch(row));
        }
        [Test]
        public void TestOutsideMetricRangeCondition()
        {
            var c = new MetricRangeCondition(MetricKey.T, 20, 25);
            var row = new Row();
            row.measures.Add(new Measure(new Metric(MetricKey.T, AggregationKey.AVG), 30));
            Assert.IsFalse(c.IsMatch(row));
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
            Assert.IsFalse(c.IsMatch(new Row()));
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
            Assert.IsTrue(c.IsMatch(new Row()));
        }

        //GroupAllCondition
        [Test]
        public void TestMatchGroupAllCondition()
        {
            var c = new GroupAllCondition(new MetricGroup("Test", MetricKey.T, MetricKey.T10), 10, 20);
            var row = new Row();
            row.measures.Add(new Measure(new Metric(MetricKey.T, AggregationKey.AVG), 15));
            row.measures.Add(new Measure(new Metric(MetricKey.T10, AggregationKey.AVG), 15));
            Assert.IsTrue(c.IsMatch(new Row()));
        }
        [Test]
        public void TestNoMatchGroupAllCondition()
        {
            var c = new GroupAllCondition(new MetricGroup("Test", MetricKey.T, MetricKey.T10), 10, 20);
            var row = new Row();
            row.measures.Add(new Measure(new Metric(MetricKey.T, AggregationKey.AVG), 15));
            row.measures.Add(new Measure(new Metric(MetricKey.T10, AggregationKey.AVG), 35));
            Assert.IsFalse(c.IsMatch(new Row()));
        }

        //GroupAnyCondition
        [Test]
        public void TestMatchGroupAnyCondition()
        {
            var c = new GroupAnyCondition(new MetricGroup("Test", MetricKey.T, MetricKey.T10), 10, 20);
            var row = new Row();
            row.measures.Add(new Measure(new Metric(MetricKey.T, AggregationKey.AVG), 15));
            row.measures.Add(new Measure(new Metric(MetricKey.T10, AggregationKey.AVG), 15));
            Assert.IsTrue(c.IsMatch(new Row()));
        }
        [Test]
        public void TestNoMatchGroupAnyondition()
        {
            var c = new GroupAnyCondition(new MetricGroup("Test", MetricKey.T, MetricKey.T10), 10, 20);
            var row = new Row();
            row.measures.Add(new Measure(new Metric(MetricKey.T, AggregationKey.AVG), 15));
            row.measures.Add(new Measure(new Metric(MetricKey.T10, AggregationKey.AVG), 35));
            Assert.IsTrue(c.IsMatch(new Row()));
        }
    }
}