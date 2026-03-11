namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    using Assets.Scripts.Foenn.OLAP.Query;
    using System.Collections.Generic;

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
