using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Components.Commons.Holders;
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
