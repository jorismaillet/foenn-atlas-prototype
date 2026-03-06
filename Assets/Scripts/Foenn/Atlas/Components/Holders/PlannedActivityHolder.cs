using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap.Go;
using Assets.Scripts.Unity.Commons.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Components.Holders
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
