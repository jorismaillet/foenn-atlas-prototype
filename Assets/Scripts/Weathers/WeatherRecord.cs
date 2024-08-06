using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherRecord
    {
        public Dictionary<WeatherFieldKey, string> values = new Dictionary<WeatherFieldKey, string>();

        public WeatherRecord(List<WeatherFieldKey> datasetKeys, string rawLine)
        {
            var array = rawLine.Split(CSVLoader.STRING_SPLIT);
            for(int i = 0; i < array.Length; i++)
            {
                if(!string.IsNullOrEmpty(array[i]))
                {
                    values.Add(datasetKeys[i], array[i]);
                }
            }
        }

        public float GetFloat(WeatherFieldKey key)
        {
            return float.Parse(values[key], CultureInfo.InvariantCulture);
        }

        public string Get(WeatherFieldKey key)
        {
            return values[key];
        }

        public int GetInt(WeatherFieldKey key)
        {
            return int.Parse(values[key]);
        }
    }
}