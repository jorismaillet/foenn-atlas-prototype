using Assets.Scripts.Foenn.Atlas.Models.Activities;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings {
    public class PlanningDefinition {
        public List<PlannedActivity> plannedActivities = new List<PlannedActivity>();
        public PlanningProposal proposal;

        public PlanningDefinition(params PlannedActivity[] plannedActivities) {
            this.plannedActivities.AddRange(plannedActivities);
        }
    }
}
