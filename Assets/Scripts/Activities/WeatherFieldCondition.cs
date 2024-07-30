using Assets.Resources.Weathers;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Activities
{
    public class WeatherFieldCondition
    {
        public WeatherFieldKey key;
        public float min;
        public float max;

        public WeatherFieldCondition(WeatherFieldKey key, float min, float max)
        {
            this.key = key;
            this.min = min;
            this.max = max;
        }

        public bool Match(WeatherRecord record)
        {
            var value = WeatherDataset.Instance.GetFloat(record, key);
            return min <= value && max >= value;
        }
    }
}