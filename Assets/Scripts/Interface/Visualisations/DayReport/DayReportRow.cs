using System;
using Assets.Scripts.Components.Commons;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations.DayReport
{
    public class DayReportRow : MonoBehaviour, IElementInitializer<Row>
    {
        public TMPro.TMP_Text hour, temp, rain, wind;

        public void Initialize(Row row)
        {
            var dataset = WeatherHistoryDataset.Instance;
            hour.text = dataset.time.hour.Value(row);
            temp.text = dataset.coreFact.temperature.Value(row);
            rain.text = dataset.coreFact.rain.Value(row);
            wind.text = dataset.coreFact.windSpeed.Value(row);
        }
    }
}
