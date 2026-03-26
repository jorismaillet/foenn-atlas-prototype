using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Interface.Visualisations.Planning;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Plannings;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface.Components.Views.Selectors
{
    public class PlanningSelector : MonoBehaviour
    {
        public TMPro.TMP_Dropdown year, month, day;
        public TMPro.TMP_Dropdown post, planning;
        public PrefabsContainer reportContainer;
        public TMPro.TMP_Text reportTitle;

        private List<Planning> plannings = new List<Planning>() { Seeds.sportsPlanning, Seeds.outsideActivities };

        private void Start()
        {
            planning.ClearOptions();
            planning.AddOptions(plannings.Select(p => p.title).ToList());
        }

        public void OnCaseSelected()
        {
            var planning = plannings[this.planning.value];
            var day = int.Parse(this.day.captionText.text);
            var year = int.Parse(this.year.captionText.text);
            var month = this.month.value;
            var post = this.post.captionText.text;
            List<(Activity a, List<int> hours)> ahs = planning.plannedActivities.Select(pa => (pa.activity, WeatherQueryService.HoursForActivity(pa.activity, post, year, month, day))).ToList();
            List<ActivityHour> activityHours = Enumerable.Range(0, 23)
                .Select(h => new ActivityHour(h, ahs.Where(ah => ah.hours.Contains(h)).Select(ahs => ahs.a).ToList())).ToList();
            reportTitle.text = $"{planning.title}: results for {day}/{month}/{year} at {post}";
            reportContainer.Initialize(activityHours);
        }
    }
}
