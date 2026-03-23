using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Interface.Visualisations;
using Assets.Scripts.Interface.Visualisations.TemporalHeatmap;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface.Components.Scenarios
{
    public class TemporalHeatmapSelector : MonoBehaviour
    {
        public TMPro.TMP_Dropdown year;
        public TMPro.TMP_Dropdown post;
        public DisplayGradient gradientDisplay;
        public RawImage image;

        public void OnSelectScenario()
        {
            var gradient = new CustomGradient(WeatherHistoryDataset.Instance.coreFact.temperature, 1);
            gradientDisplay.Initialize(gradient);
            image.texture = TemporalHeatmapGenerator.BuildTemperatureTemporalHeatmap(
                WeatherQueryService.HoursTempForYear(post.captionText.text, int.Parse(year.captionText.text)),
                gradient
            );
        }
    }
}
