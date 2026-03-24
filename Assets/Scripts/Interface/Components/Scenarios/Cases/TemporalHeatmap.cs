using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Interface.Visualisations;
using Assets.Scripts.Interface.Visualisations.TemporalHeatmap;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface.Components.Scenarios
{
    public class TemporalHeatmap : MonoBehaviour
    {
        public TMPro.TMP_Dropdown year;
        public TMPro.TMP_Dropdown post;
        public DisplayGradient gradientDisplay;
        public RawImage image;

        public void OnSelectScenario()
        {
            var gradient = new CustomGradient(WeatherHistoryDataset.Instance.coreFact.temperature, 1);
            gradientDisplay.Initialize(gradient);
            var temps = WeatherQueryService.HoursTempForYear(post.captionText.text, int.Parse(year.captionText.text));
            if(temps.Count % 24 != 0)
            {
                Debug.LogWarning("Invalid data for temporal heatmap, hours in year should be a multiple of 24");
                return;
            }
            image.texture = TemporalHeatmapGenerator.BuildTemperatureTemporalHeatmap(
                temps,
                gradient
            );
        }
    }
}
