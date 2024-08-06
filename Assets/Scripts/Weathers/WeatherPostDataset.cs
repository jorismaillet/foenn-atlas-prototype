using Assets.Resources.Activities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherPostDataset
    {
        public int id;
        public string post;
        public List<WeatherFieldKey> availableKeys;
        public List<WeatherRecord> records;

        public WeatherPostDataset(int id, string post, List<WeatherRecord> records, List<WeatherFieldKey> availableKeys)
        {
            this.id = id;
            this.post = post;
            this.availableKeys = availableKeys;
            this.records = records;
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
    }
}