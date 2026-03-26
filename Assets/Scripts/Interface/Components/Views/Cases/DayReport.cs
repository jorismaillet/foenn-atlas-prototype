using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Cases
{
    public class DayReport : MonoBehaviour
    {
        public TMPro.TMP_Dropdown year, month, day;
        public TMPro.TMP_Dropdown post;
        public PrefabsContainer reportContainer;
        public TMPro.TMP_Text reportTitle;

        public void OnCaseSelected()
        {
            var day = int.Parse(this.day.captionText.text);
            var year = int.Parse(this.year.captionText.text);
            var month = this.month.value;
            reportTitle.text = $"Observations for {post.captionText.text} on {day} {this.month.captionText.text} {year}";
            var rows = WeatherQueryService.DayObservationsForPost(
                post.captionText.text,
                day,
                month,
                year
            );
            reportContainer.Initialize(rows);
        }
    }
}
