using Assets.Scripts.Foenn.Atlas.Models;
using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Locations;

namespace Assets.Editor.Tests
{
    public class LocationTest
    {
        public void TestActivityCreation()
        {
            var brest = new PointLocation("Brest", new GeoPoint(48.3904, -4.4861));
            Assert.AreEqual("Test activity", activity.name);
        }
        [Test]
    }
}