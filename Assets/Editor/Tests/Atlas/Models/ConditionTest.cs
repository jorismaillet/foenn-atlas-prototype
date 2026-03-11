namespace Assets.Editor.Tests.Atlas.Models
{
    using Assets.Scripts.Foenn.Atlas.Models.Activities;
    using Assets.Scripts.Foenn.Atlas.Models.Condition;
    using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using NUnit.Framework;

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
            var row = new Row() { time = TimeField.AAAAMMJJHH("2023091508") };
            Assert.IsTrue(c.IsMatch(row));
        }

        [Test]
        public void TestOutsideHourRangeCondition()
        {
            var c = new HourRangeCondition(8, 10);
            var row = new Row() { time = TimeField.AAAAMMJJHH("2023091510") };
            Assert.IsFalse(c.IsMatch(row));
        }

        //MetricRangeCondition
        [Test]
        public void TestInsideMetricRangeCondition()
        {
            var c = new MetricRangeCondition(WeatherFact.temperature, 20, 25);
            var row = new Row();
            row.values.Add(WeatherFact.temperature, new Measure(WeatherFact.temperature, 20));
            Assert.IsTrue(c.IsMatch(row));
        }

        [Test]
        public void TestOutsideMetricRangeCondition()
        {
            var c = new MetricRangeCondition(WeatherFact.temperature, 20, 25);
            var row = new Row();
            row.values.Add(WeatherFact.temperature, new Measure(WeatherFact.temperature, 30));
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

        //GroupAllCondition
        [Test]
        public void TestMatchGroupAllCondition()
        {
            var c = new GroupAllCondition(new MetricGroup("Test", WeatherFact.temperature, WeatherFact.temperature_10), 10, 20);
            var row = new Row();
            row.values.Add(WeatherFact.temperature, new Measure(WeatherFact.temperature, 15));
            row.values.Add(WeatherFact.temperature_10, new Measure(WeatherFact.temperature_10, 15));
            Assert.IsTrue(c.IsMatch(row));
        }

        [Test]
        public void TestNoMatchGroupAllCondition()
        {
            var c = new GroupAllCondition(new MetricGroup("Test", WeatherFact.temperature, WeatherFact.temperature_10), 10, 20);
            var row = new Row();
            row.values.Add(WeatherFact.temperature, new Measure(WeatherFact.temperature, 15));
            row.values.Add(WeatherFact.temperature_10, new Measure(WeatherFact.temperature_10, 35));
            Assert.IsFalse(c.IsMatch(row));
        }

        //GroupAnyCondition
        [Test]
        public void TestMatchGroupAnyCondition()
        {
            var c = new GroupAnyCondition(new MetricGroup("Test", WeatherFact.temperature, WeatherFact.temperature_10), 10, 20);
            var row = new Row();
            row.values.Add(WeatherFact.temperature, new Measure(WeatherFact.temperature, 15));
            row.values.Add(WeatherFact.temperature_10, new Measure(WeatherFact.temperature_10, 15));
            Assert.IsTrue(c.IsMatch(row));
        }

        [Test]
        public void TestNoMatchGroupAnyondition()
        {
            var c = new GroupAnyCondition(new MetricGroup("Test", WeatherFact.temperature, WeatherFact.temperature_10), 10, 20);
            var row = new Row();
            row.values.Add(WeatherFact.temperature, new Measure(WeatherFact.temperature, 15));
            row.values.Add(WeatherFact.temperature_10, new Measure(WeatherFact.temperature_10, 35));
            Assert.IsTrue(c.IsMatch(row));
        }
    }
}
