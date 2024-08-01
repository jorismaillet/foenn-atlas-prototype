using Assets.Resources.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Resources.Weathers
{
    public class WeatherPostDataset
    {
        private WeatherDataset dataset;
        public int id;
        public string post;
        public List<WeatherFieldKey> availableKeys;

        public WeatherPostDataset(WeatherDataset dataset, int id, string post, List<WeatherFieldKey> availableKeys)
        {
            this.dataset = dataset;
            this.id = id;
            this.post = post;
            this.availableKeys = availableKeys;
        }

        public IEnumerable<WeatherRecord> Records()
        {
            return dataset.Records(id);
        }

        public bool HasRecordsFor(Activity activity)
        {
            return activity.hourlyConditions.Union(activity.cumulatedHourConditions).All(condition =>
            {
                return condition.keys.Any(key =>
                {
                    return availableKeys.Contains(key);
                });
            });
        }

        public int AverageTemperature()
        {
            return (int)(Records().Select(entry => GetFloat(entry, WeatherFieldKey.T)).Average());
        }

        private float GetFloat(WeatherRecord record, WeatherFieldKey key)
        {
            return record.GetFloat(Array.IndexOf(dataset.availableKeys, key));
        }
    }
}