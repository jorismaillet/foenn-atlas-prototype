using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherRecord
    {
        public Dictionary<WeatherFieldKey, string> values = new Dictionary<WeatherFieldKey, string>();

        public WeatherRecord()
        {

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