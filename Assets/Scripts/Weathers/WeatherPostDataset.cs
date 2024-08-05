using Assets.Resources.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
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
        public List<WeatherRecord> records;

        public WeatherPostDataset(WeatherDataset dataset, int id, string post, List<WeatherFieldKey> availableKeys)
        {
            this.dataset = dataset;
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

        public async Task Load(int department, int year)
        {
            var csv = new CSVLoader();
            var remainingLiens = await csv.Load(WeatherDataset.WeatherFileName(department, year), id);
            var rawRecords = remainingLiens.Select(line => new WeatherRecord(availableKeys, line));
            records = rawRecords.Where(record => record.Get(WeatherFieldKey.AAAAMMJJHH).StartsWith(year.ToString())).ToList();
        }
    }
}