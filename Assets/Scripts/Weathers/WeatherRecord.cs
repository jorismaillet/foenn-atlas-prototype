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

        public string Get(int index)
        {
            return values[index];
        }

        public int GetInt(int index)
        {
            return int.Parse(Get(index));
        }

        public float GetFloat(int index)
        {
            return float.Parse(Get(index), CultureInfo.InvariantCulture);
        }
    }
}