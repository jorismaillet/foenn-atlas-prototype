using Assets.Scripts.Foenn.Atlas.Models.Activities;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    public class PlannedActivity
    {
        public PlanningDefinition planning;
        public Activity activity;

        public PlannedActivity(PlanningDefinition planning, Activity activity)
        {
            this.planning = planning;
            this.activity = activity;
        }
    }
}