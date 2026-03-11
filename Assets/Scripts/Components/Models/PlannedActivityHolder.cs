using Assets.Scripts.Components.Commons.Holders;
using Assets.Scripts.Models.Locations;
using Assets.Scripts.Models.Plannings;

namespace Assets.Scripts.Components.Models
{
    public class PlannedActivityHolder : Holder<PlannedActivity>
    {
        public void SetActivity(ActivityHolder holder)
        {
            holder.Initialize(element.activity);
        }

        public void SetILocation(ILocationHolder holder)
        {
            holder.Initialize(element.location);
        }

        public void SetPointLocation(PointLocationHolder holder)
        {
            if (element.location is PointLocation pointLocation)
            {
                holder.Initialize(pointLocation);
            }
        }
    }
}
