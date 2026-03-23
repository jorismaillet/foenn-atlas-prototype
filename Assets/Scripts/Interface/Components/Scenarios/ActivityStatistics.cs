using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Components.Visualisations.Heatmap;
using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Interface.Visualisations;
using Assets.Scripts.Services;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Scenarios
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
            displayGradient.Initialize(CustomGradient.CumulativeYearHours);
            heatmapContainer.SetMeasures(geoMeasures, CustomGradient.CumulativeYearHours);
            pointmapContainer.Initialize(geoMeasures);
        }
    }
}
