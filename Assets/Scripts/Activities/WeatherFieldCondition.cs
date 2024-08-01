using Assets.Resources.Weathers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Activities
{
    public class WeatherFieldCondition
    {
        public List<WeatherFieldKey> keys;
        public float min;
        public float max;

        public WeatherFieldCondition(WeatherFieldKey key, float min, float max)
        {
            this.keys = new List<WeatherFieldKey>() { key };
            this.min = min;
            this.max = max;
        }
        public WeatherFieldCondition(List<WeatherFieldKey> keys, float min, float max)
        {
            this.keys = keys;
            this.min = min;
            this.max = max;
        }

        public bool Match(WeatherRecord record, List<WeatherFieldKey> availableKeys)
        {
            return keys.Intersect(availableKeys).Any(key =>
            {
                try
                {
                    var value = WeatherDataset.Instance.GetFloat(record, key);
                    return min <= value && max >= value;
                }
                catch(FormatException e)
                {
                    return false;
                }
            });
        }
    }
}