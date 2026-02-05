using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using NUnit.Framework;

namespace Assets.Editor.Tests
{
    public class PlanningTest
    {
        [Test]
        public void TestActivityCreation()
        {
            var planningSportif = new PlanningDefinition();
            Assert.AreEqual("Test activity", activity.name);
        }
        [Test]
        public void TestActivityCreation()
        {
            planningSportif.plannedActivities.Add(new PlannedActivity(randonee, procheMaison));
            Assert.AreEqual("Test activity", activity.name);
        }
    }
}