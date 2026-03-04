using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Locations;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    public class PlannedActivity
    {
        public Activity activity;
        public ILocation location;

        public PlannedActivity(Activity activity, ILocation location)
        {
            this.activity = activity;
            this.location = location;
        }
    }
}