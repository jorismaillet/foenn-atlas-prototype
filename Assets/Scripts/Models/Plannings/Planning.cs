using System.Collections.Generic;

namespace Assets.Scripts.Models.Plannings
{
    public class Planning
    {
        public string title;

        public readonly List<PlannedActivity> plannedActivities = new List<PlannedActivity>();

        public readonly List<ActivityProposal> activityProposals;

        public readonly List<PlanningRow> planningRows;

        public Planning(string title, params PlannedActivity[] plannedActivities)
        {
            this.title = title;
            this.plannedActivities = new List<PlannedActivity>(plannedActivities);
        }
    }
}
