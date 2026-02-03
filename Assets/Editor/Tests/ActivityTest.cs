using NUnit.Framework;
using Assets.Scripts.Foenn.Atlas.Models.Activities;

public class ActivityTest
{
    [Test]
    public void TestActivityCreation()
    {
        var activity = new Activity("Test activity");
        Assert.AreEqual("Test activity", activity.name);
    }
}
