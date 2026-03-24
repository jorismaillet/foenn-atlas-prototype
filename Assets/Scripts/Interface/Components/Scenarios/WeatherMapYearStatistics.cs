using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Components.Visualisations.Heatmap;
using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Interface.Visualisations;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Scenarios
{
    public class WeatherMapYearStatistics : MonoBehaviour
    {
        public PrefabsContainer pointmapContainer;
        public HeatmapWorldOverlay heatmapContainer;

        public TMPro.TMP_Dropdown scenarioDropdown;
        public TMPro.TMP_Dropdown yearDropdown;
        public TMPro.TMP_Dropdown metricDropdown;
        public TMPro.TMP_Dropdown statTypeDropdown;
        public DisplayGradient displayGradient;

        private const string hoursRainScenarioText = "Hours without rain";
        private const string statScenarioText = "Metric statistics";

        public void OnCaseSelected()
        {
            switch (scenarioDropdown.captionText.text)
            {
                case hoursRainScenarioText:
                    SetHoursRainScenario();
                    break;
                case statScenarioText:
                    SetFieldStatScenario();
                    break;
            }
        }

        public void SetHoursRainScenario()
        {
            var year = int.Parse(yearDropdown.captionText.text);
            var geoMeasures = WeatherQueryService.HoursWithoutRain(year);
            var field = WeatherHistoryDataset.Instance.coreFact.rain;
            var gradient = new CustomGradient(CustomGradient.CreateDroughtGradient(), "Hours", "h", 4100, 4700, 1);
            displayGradient.Initialize(gradient);
            heatmapContainer.SetMeasures(geoMeasures, gradient);
            pointmapContainer.Initialize(geoMeasures);
        }

        public void SetFieldStatScenario()
        {
            var metricName = metricDropdown.captionText.text;
            var fact = WeatherHistoryDataset.Instance.coreFact;
            var field = fact.Mappings.Find(m => m.targetField.fieldName.Equals(metricName)).targetField;
            var year = int.Parse(yearDropdown.captionText.text);
            var geoMeasures = WeatherQueryService.YearMeasure(year, field, statTypeDropdown.captionText.text switch
            {
                "Minimum" => FieldAggregation.Min,
                "Maximum" => FieldAggregation.Max,
                _ => FieldAggregation.Avg
            });
            var gradient = new CustomGradient(field);
            displayGradient.Initialize(gradient);
            heatmapContainer.SetMeasures(geoMeasures, gradient);
            pointmapContainer.Initialize(geoMeasures);
        }

        private void Start()
        {
            scenarioDropdown.ClearOptions();
            metricDropdown.ClearOptions();
            scenarioDropdown.AddOptions(new List<string>() { hoursRainScenarioText, statScenarioText });
            var coreFact = WeatherHistoryDataset.Instance.coreFact;
            metricDropdown.AddOptions(new List<string> { coreFact.temperature.fieldName, coreFact.rain.fieldName, coreFact.windSpeed.fieldName }.ToList());
            statTypeDropdown.ClearOptions();
            statTypeDropdown.AddOptions(new List<string> { "Minimum", "Maximum", "Average" }.ToList());
            scenarioDropdown.onValueChanged.AddListener((_) =>
            {
                if(scenarioDropdown.captionText.text.Equals(hoursRainScenarioText))
                {
                    Main.selectedScenario.Set(ScenarioKey.HOUR_NO_RAIN);
                } else
                {
                    Main.selectedScenario.Set(ScenarioKey.FIELD_STATS);
                }
            });
        }
    }
}
