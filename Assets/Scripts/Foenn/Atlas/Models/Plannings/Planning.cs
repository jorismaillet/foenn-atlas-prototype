using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    public class Planning
    {
        public List<PlannedActivity> plannedActivities = new List<PlannedActivity>();
        public List<ActivityProposal> activityProposals;
        public List<PlanningRow> planningRows;
    }
}
