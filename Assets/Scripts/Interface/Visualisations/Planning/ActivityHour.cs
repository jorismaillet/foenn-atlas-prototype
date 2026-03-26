using System.Collections.Generic;
using Assets.Scripts.Models.Activities;

namespace Assets.Scripts.Interface.Visualisations.Planning
{
    public class ActivityHour
    {
        public int hour;
        public List<Activity> activities;

        public ActivityHour(int hour, List<Activity> activities)
        {
            this.hour = hour;
            this.activities = activities;
        }
    }
}
