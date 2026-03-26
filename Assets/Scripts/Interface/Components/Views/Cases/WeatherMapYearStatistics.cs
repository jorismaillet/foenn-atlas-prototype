using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Interface.Components.Views.Navigation;
using Assets.Scripts.Interface.Visualisations;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Cases
{
    public class WeatherMapYearStatistics : MonoBehaviour
    {
        public PrefabsContainer pointmapContainer;
        public HeatmapWorldOverlay heatmapContainer;

        public TMPro.TMP_Dropdown subCaseDropDown;
        public TMPro.TMP_Dropdown yearDropdown;
        public TMPro.TMP_Dropdown metricDropdown;
        public TMPro.TMP_Dropdown statTypeDropdown;
        public DisplayGradient displayGradient;

        private const string hourRainSubcaseText = "Hours without rain";
        private const string metricStatsSubcaseText = "Metric statistics";

        public void OnCaseSelected()
        {
            switch (subCaseDropDown.captionText.text)
            {
                case hourRainSubcaseText:
                    SetHoursRainCase();
                    break;
                case metricStatsSubcaseText:
                    SetFieldStatCase();
                    break;
            }
        }

        public void SetHoursRainCase()
        {
            var year = int.Parse(yearDropdown.captionText.text);
            var geoMeasures = WeatherQueryService.HoursWithoutRain(year);
            var field = WeatherHistoryDataset.Instance.coreFact.rain;
            var gradient = new CustomGradient(CustomGradient.CreateDroughtGradient(), "Hours", "h", 4100, 4700, 1);
            displayGradient.Initialize(gradient);
            heatmapContainer.SetMeasures(geoMeasures, gradient);
            pointmapContainer.Initialize(geoMeasures);
        }

        public void SetFieldStatCase()
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
            subCaseDropDown.ClearOptions();
            metricDropdown.ClearOptions();
            subCaseDropDown.AddOptions(new List<string>() { hourRainSubcaseText, metricStatsSubcaseText });
            var coreFact = WeatherHistoryDataset.Instance.coreFact;
            metricDropdown.AddOptions(new List<string> { coreFact.temperature.fieldName, coreFact.rain.fieldName, coreFact.windSpeed.fieldName }.ToList());
            statTypeDropdown.ClearOptions();
            statTypeDropdown.AddOptions(new List<string> { "Minimum", "Maximum", "Average" }.ToList());
            subCaseDropDown.onValueChanged.AddListener((_) =>
            {
                if(subCaseDropDown.captionText.text.Equals(hourRainSubcaseText))
                {
                    Main.selectedView.Set(ViewKey.HOUR_NO_RAIN);
                } else
                {
                    Main.selectedView.Set(ViewKey.FIELD_STATS);
                }
            });
        }
    }
}
