using System.Collections.Generic;
using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Models.Plannings
{
    public class PlanningRow
    {
        public readonly Row row;

        public readonly List<ActivitySuit> activitySuits;

        public PlanningRow(Row row, List<ActivitySuit> activitySuits)
        {
            this.row = row;
            this.activitySuits = activitySuits;
        }
    }
}
