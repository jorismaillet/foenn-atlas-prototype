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

        public WeatherDataset(string fileName, int year, int department, string fileText, List<Activity> activities, List<WeatherRecordFieldKey> keysToLoad)
        {
            this.year = year;
            this.id = department;
            //Debug.Log($"Load CSV {fileName}");
            var csv = new CSVLoader().LoadCSV(fileText, year, keysToLoad);
            //Debug.Log($"Done");
            foreach (var group in csv.lines.GroupBy(record => record.Get(WeatherRecordFieldKey.NOM_USUEL)))
            {
                var availableKeys = group.First().values.Keys;
                if(activities.All(activity => HasRecordsFor(availableKeys, activity)))
                {
                    //Debug.Log($"Add post {group.Key}");
                    posts.Add(new WeatherPostDataset(group.Key, department, group));
                }
            };
        }

        public bool HasRecordsFor(IEnumerable<WeatherRecordFieldKey> availableKeys, Activity activity)
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
            string prefix = department switch
            {
                _ => "H_"
            };
            string dateKey = department switch
            {
                83 or 09 or 31 or 59 or 64 or 80 or 76 or 27 or 14 or 50 or 62 or 66 or 85 or 65 or 75 or 12 or 48 or 7 => year switch
                {
                    2024 => "latest-2024-2025",
                    2023 => "previous-2020-2023",
                    _ => throw new Exception($"Year not supported: {year}")
                },
                _ => year switch
                {
                    2024 => "latest-2023-2024",
                    2023 => "latest-2023-2024",
                    2022 => "previous-2020-2022",
                    2021 => "previous-2020-2022",
                    2020 => "previous-2020-2022",
                    _ => throw new Exception($"Year not supported: {year}")
                }
            };
            var dptString = department < 10 ? "0" + department.ToString() : department.ToString();
            var suffix = department switch
            {
                _ => ""
            };
            return $"{prefix}{dptString}_{dateKey}{suffix}";
        }
    }
}