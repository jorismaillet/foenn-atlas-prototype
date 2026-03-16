using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Condition.Definitions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using NUnit.Framework;

namespace Assets.Editor.Tests.Models
{
    public class ActivityTest
    {
        [Test]
        public void TestActivityCreation()
        {
            var dataset = WeatherHistoryDataset.Instance;
            var hourCondition = new IntRangeCondition(dataset.time.hour, 14, 16);
            var tempCondition = new FloatRangeCondition(dataset.coreFact.temperature, 20, 30);
            var rainCondition = new FloatRangeCondition(dataset.coreFact.rain, 0, 0);
            var activity = new Activity("Beach", hourCondition, tempCondition, rainCondition);
            var row = new Row();
            row.values[dataset.time.hour] = 15;
            row.values[dataset.coreFact.rain] = 0;
            row.values[dataset.coreFact.temperature] = 25F;
            Assert.IsTrue(activity.conditions.IsMatch(row));
        }
    }
}
