using Assets.Scripts.Foenn.Engine.Execution;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    public class PlanningRow
    {
        public Row row;
        public List<ActivitySuit> activitySuits;

        public PlanningRow(Row row, List<ActivitySuit> activitySuits)
        {
            this.row = row;
            this.activitySuits = activitySuits;
        }
    }
}
