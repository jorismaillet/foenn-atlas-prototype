using Assets.Resources.Activities;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherDataset
    {
        public int id;

        public const string WEATHER_PATH = "Weathers/";

        public List<WeatherPostDataset> posts = new List<WeatherPostDataset>();

        public int year;

        public WeatherDataset(int year, int department, string fileText, List<Activity> activities, List<WeatherFieldKey> keysToLoad)
        {
            Debug.Log("A");
            this.year = year;
            this.id = department;
            var csv = new CSVLoader().LoadCSV(fileText, year, keysToLoad);
            foreach (var group in csv.lines.GroupBy(record => record.Get(WeatherFieldKey.NOM_USUEL)))
            {
                var availableKeys = group.First().values.Keys.ToList();
                Debug.Log(group.Key);
                if(activities.All(activity => HasRecordsFor(availableKeys, activity))) {
                    posts.Add(new WeatherPostDataset(group.Key, group.ToList()));
                }
            };
            Debug.Log("B");
        }

        public bool HasRecordsFor(List<WeatherFieldKey> availableKeys, Activity activity)
        {
            return activity.hourlyConditions.Union(activity.cumulatedHourConditions).All(condition =>
            {
                return condition.keys.Any(key =>
                {
                    return availableKeys.Contains(key);
                });
            });
        }

        public WeatherPostDataset Post(string city)
        {
            return posts.FirstOrDefault(post => post.post.IndexOf(city, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public static string WeatherFileName(int department, int year)
        {
            return $"{WEATHER_PATH}{FileName(department, year)}";
        }


        private static string FileName(int department, int year)
        {
            string dateKey = year switch
            {
                2024 => "latest-2023-2024",
                2023 => "latest-2023-2024",
                2022 => "previous-2020-2022",
                2021 => "previous-2020-2022",
                2020 => "previous-2020-2022",
                _ => throw new Exception("Year not supported")
            };
            var dptString = department < 10 ? "0" + department.ToString() : department.ToString();
            return $"H_{dptString}_{dateKey}";
        }
    }
}