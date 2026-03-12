using System.Collections.Generic;
using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Plannings
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
