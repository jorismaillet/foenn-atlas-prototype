using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Locations;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    public class PlannedActivity
    {
        public Activity activity;
        public Location location;

        public PlannedActivity(Activity activity, Location location)
        {
            this.activity = activity;
            this.location = location;
        }
    }
}