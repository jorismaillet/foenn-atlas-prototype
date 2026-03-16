using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Locations;

namespace Assets.Scripts.Models.Plannings
{
    public class PlannedActivity
    {
        public readonly Activity activity;

        public ILocation location;

        public PlannedActivity(Activity activity, ILocation location)
        {
            this.activity = activity;
            this.location = location;
        }
    }
}
