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

        public WeatherPostDataset(int id, string post, List<WeatherFieldKey> availableKeys)
        {
            this.id = id;
            this.post = post;
            this.availableKeys = availableKeys;
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

        public void Load(int year, string textFile, List<WeatherFieldKey> datasetKeys)
        {
            var csv = new CSVLoader(id).LoadCSV(textFile); //TODO Weather dataset should load all records and group them by post
            var rawRecords = csv.lines.Select(line => new WeatherRecord(datasetKeys, line));
            records = rawRecords.Where(record => record.Get(WeatherFieldKey.AAAAMMJJHH).StartsWith(year.ToString())).ToList();
        }
    }
}