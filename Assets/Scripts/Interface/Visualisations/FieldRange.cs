using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.Interface.Visualisations
{
    public class FieldRange
    {
        private static readonly Dictionary<Field, (float minValue, float maxValue)> Ranges =
            new Dictionary<Field, (float minValue, float maxValue)>()
            {
                { WeatherHistoryDataset.Instance.coreFact.temperature, (-10f, 40f) },
                { WeatherHistoryDataset.Instance.coreFact.rain, (0f, 40f) },
                { WeatherHistoryDataset.Instance.coreFact.windSpeed, (0f, 30f) }
            };

        public float minValue;
        public float maxValue;

        public FieldRange(Field field, float multiplier)
        {
            var range = Ranges[field];
            minValue = range.minValue * multiplier;
            maxValue = range.maxValue * multiplier;
        }
    }
}
