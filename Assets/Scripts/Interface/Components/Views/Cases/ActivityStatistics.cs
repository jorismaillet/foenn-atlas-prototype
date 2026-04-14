using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Interface.Components.Views.Selectors;
using Assets.Scripts.Interface.Visualisations;
using Assets.Scripts.Services;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Cases
{
    public class ActivityStatistics : MonoBehaviour
    {
        public PrefabsContainer pointmapContainer;

        public HeatmapWorldOverlay heatmapContainer;

        public TMP_Dropdown yearDropdown;

        public ActivitySelector activitySelector;

        public DisplayGradient displayGradient;

        public void OnCaseSelected()
        {
            var geoMeasures = WeatherQueryService.HoursOfActivity(activitySelector.SelectedActivity(), int.Parse(yearDropdown.captionText.text));
            var relativeGradient = new CustomGradient(CustomGradient.CreateCumulativeYearHoursGradient(), "Hours", "h", 0f, geoMeasures.Max(m => m.value), 1);
            displayGradient.Initialize(relativeGradient);
            heatmapContainer.SetMeasures(geoMeasures, relativeGradient);
            pointmapContainer.Initialize(geoMeasures);
        }
    }
}
