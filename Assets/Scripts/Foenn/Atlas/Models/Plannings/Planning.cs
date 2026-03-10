namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    using System.Collections.Generic;

    public class Planning
    {
        public string title;

        public List<PlannedActivity> plannedActivities = new List<PlannedActivity>();

        public List<ActivityProposal> activityProposals;

        public List<PlanningRow> planningRows;

        public Planning(string title, params PlannedActivity[] plannedActivities)
        {
            this.title = title;
            this.plannedActivities = new List<PlannedActivity>(plannedActivities);
        }
    }
}
