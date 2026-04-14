using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Activities;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Selectors
{
    public class ActivitySelector : MonoBehaviour
    {
        public TMPro.TMP_Dropdown activitiesDropdown;

        private List<Activity> activities = new List<Activity>();

        private void Start()
        {
            activities = new List<Activity>() {
                Seeds.swimming,
                Seeds.kayak,
                Seeds.beach,
                Seeds.biking,
                Seeds.gardening,
                Seeds.tennis,
                Seeds.cityPromenade,
                Seeds.hiking,
                Seeds.dinner
            };

            activitiesDropdown.ClearOptions();
            activitiesDropdown.AddOptions(activities.Select(a => a.name).ToList());
        }

        public Activity SelectedActivity()
        {
            return activities[activitiesDropdown.value];
        }
    }
}
