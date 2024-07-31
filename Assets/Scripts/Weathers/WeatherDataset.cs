using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Resources.Weathers
{
    public class WeatherDataset
    {
        public static WeatherDataset Instance;

        private const string WEATHER_PATH = "Weathers/";

        public WeatherFieldKey[] keys;
        public string[] posts;
        public Dictionary<string, List<WeatherFieldKey>> fieldsByPost = new Dictionary<string, List<WeatherFieldKey>>();

        public int year;
        public int department;

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
            return $"H_{department}_{dateKey}";
        }

        public static WeatherDataset Load(int department, int year)
        {
            Instance = new WeatherDataset(department, year);
            return Instance;
        }

        public string Post(string city)
        {
            return posts.FirstOrDefault(post => post.IndexOf(city, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public WeatherDataset(int department, int year)
        {
            this.year = year;
            this.department = department;
            var csv = new CSVLoader(WeatherFileName(department, year));
            keys = csv.header.Select(rawKey => Enum.Parse<WeatherFieldKey>(rawKey)).ToArray();
            posts = Records(csv).GroupBy(record => Get(record, WeatherFieldKey.NOM_USUEL)).Select(group => {
                var availableKeys = new List<WeatherFieldKey>();
                var firstRow = group.First();
                for (int i = 0; i < firstRow.values.Count(); i++)
                {
                    if(firstRow.values[i] != null)
                    {
                        availableKeys.Add(keys[i]);
                    }
                }
                fieldsByPost.Add(group.Key, availableKeys);
                return group.Key;
            }).ToArray();
        }

        public static string WeatherFileName(int department, int year)
        {
            return $"{WEATHER_PATH}{FileName(department, year)}";
        }

        private IEnumerable<WeatherRecord> Records(CSVLoader csv)
        {
            return csv.remainingLines.Select(line => new WeatherRecord(line)).Where(record => Get(record, WeatherFieldKey.AAAAMMJJHH).StartsWith(year.ToString()));
        }

        public IEnumerable<WeatherRecord> Records()
        {
            return Records(new CSVLoader(WeatherFileName(department, year)));
        }

        public IEnumerable<WeatherRecord> Records(string post)
        {
            return Records().Where(record => Get(record, WeatherFieldKey.NOM_USUEL).Equals(post));
        }

        public float GetFloat(WeatherRecord record, WeatherFieldKey key)
        {
            return record.GetFloat(Array.IndexOf(keys, key));
        }

        public string Get(WeatherRecord record, WeatherFieldKey key)
        {
            return record.Get(Array.IndexOf(keys, key));
        }

        public int GetInt(WeatherRecord record, WeatherFieldKey key)
        {
            return record.GetInt(Array.IndexOf(keys, key));
        }

        public int EntriesQuantity()
        {
            return Records().Count();
        }

        public int PostsQuantity()
        {
            return Records().GroupBy(entry => Get(entry, WeatherFieldKey.NUM_POSTE)).Count();
        }
    }
}