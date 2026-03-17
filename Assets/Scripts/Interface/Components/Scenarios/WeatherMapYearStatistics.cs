using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Components.Visualisations.Heatmap;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
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

        private const string hoursRainScenarioText = "Hours without rain";
        private const string recordScenarioText = "Metric records";

        public void OnCaseSelected()
        {
            switch (scenarioDropdown.captionText.text)
            {
                case hoursRainScenarioText:
                    SetHoursRainScenario();
                    break;
                case recordScenarioText:
                    SetFieldRecordScenario();
                    break;
            }
        }

        public void SetHoursRainScenario()
        {
            var year = int.Parse(yearDropdown.captionText.text);
            var geoMeasures = WeatherQueryService.HoursWithoutRain(year);
            heatmapContainer.SetMeasures(geoMeasures);
            pointmapContainer.Initialize(geoMeasures);
        }

        public void SetFieldRecordScenario()
        {
            var metricName = metricDropdown.captionText.text;
            var fact = WeatherHistoryDataset.Instance.coreFact;
            var field = fact.Mappings.Find(m => m.targetField.fieldName.Equals(metricName)).targetField;
            var year = int.Parse(yearDropdown.captionText.text);
            var geoMeasures = WeatherQueryService.MaxYearMeasure(year, field);
            heatmapContainer.SetMeasures(geoMeasures);
            pointmapContainer.Initialize(geoMeasures);
        }

        private void Start()
        {
            scenarioDropdown.ClearOptions();
            metricDropdown.ClearOptions();
            scenarioDropdown.AddOptions(new List<string>() { hoursRainScenarioText, recordScenarioText });
            metricDropdown.AddOptions(WeatherHistoryDataset.Instance.coreFact.Mappings.Select(m => m.targetField.fieldName).ToList());
            scenarioDropdown.onValueChanged.AddListener((_) =>
            {
                if(scenarioDropdown.captionText.text.Equals(hoursRainScenarioText))
                {
                    Main.selectedScenario.Set(ScenarioKey.HOUR_NO_RAIN);
                } else
                {
                    Main.selectedScenario.Set(ScenarioKey.FIELD_RECORD);
                }
            });
        }
    }
}
