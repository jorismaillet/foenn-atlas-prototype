using System;
using System.Globalization;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherRecord
    {
        public string[] values;

        public WeatherRecord(string rawLine)
        {
            values = rawLine.Split(CSVLoader.STRING_SPLIT);
        }

        private string Get(int index)
        {
            return values[index];
        }

        public float GetFloat(WeatherFieldKey key)
        {
            return float.Parse(Get(Array.IndexOf(WeatherDataset.Instance.availableKeys, key)));
        }

        public string Get(WeatherFieldKey key)
        {
            return Get(Array.IndexOf(WeatherDataset.Instance.availableKeys, key));
        }

        public int GetInt(WeatherFieldKey key)
        {
            return int.Parse(Get(Array.IndexOf(WeatherDataset.Instance.availableKeys, key)));
        }
    }
}