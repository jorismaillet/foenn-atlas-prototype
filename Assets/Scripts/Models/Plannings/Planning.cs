using System.Collections.Generic;

namespace Assets.Scripts.Models.Plannings
{
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
