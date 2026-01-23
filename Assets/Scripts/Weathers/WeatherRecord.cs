using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherRecord
    {
        public Dictionary<WeatherRecordFieldKey, string> values = new Dictionary<WeatherRecordFieldKey, string>();

        public WeatherRecord()
        {

        }

        public float GetFloat(WeatherRecordFieldKey key)
        {
            return float.Parse(values[key], CultureInfo.InvariantCulture);
        }

        public string Get(WeatherRecordFieldKey key)
        {
            return values[key];
        }

        public int GetInt(WeatherRecordFieldKey key)
        {
            return int.Parse(values[key]);
        }
    }
}