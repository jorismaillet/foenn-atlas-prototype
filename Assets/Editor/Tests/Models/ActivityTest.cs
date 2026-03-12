using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Condition;
using Assets.Scripts.Models.Condition.Definitions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts;
using Assets.Scripts.OLAP.Engine;
using NUnit.Framework;

namespace Assets.Editor.Tests.Models
{
    public class ActivityTest
    {
        [Test]
        public void TestActivityCreation()
        {
            var hourCondition = new IntRangeCondition(TimeDimension.hour, 14, 16);
            var tempCondition = new FloatRangeCondition(WeatherFact.temperature, 20, 30);
            var rainCondition = new FloatRangeCondition(WeatherFact.rain, 0, 0);
            var activity = new Activity("Beach", hourCondition, tempCondition, rainCondition);
            var row = new Row();
            row.values[TimeDimension.hour] = 15;
            row.values[WeatherFact.rain] = 0;
            row.values[WeatherFact.temperature] = 25F;
            Assert.IsTrue(activity.conditions.IsMatch(row));
        }
    }
}
