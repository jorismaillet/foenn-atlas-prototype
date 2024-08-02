using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Resources.Weathers
{
    public class WeatherDataset
    {
        public static WeatherDataset Instance;

        public const string WEATHER_PATH = "Weathers/";

        public WeatherFieldKey[] availableKeys;
        public Dictionary<int, WeatherPostDataset> posts = new Dictionary<int, WeatherPostDataset>();

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

        public WeatherPostDataset Post(string city)
        {
            return posts.Values.FirstOrDefault(post => post.post.IndexOf(city, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public WeatherDataset(int department, int year)
        {
            this.year = year;
            this.department = department;

            var csv = new CSVLoader(WeatherFileName(department, year));
            availableKeys = csv.header.Select(rawKey => Enum.Parse<WeatherFieldKey>(rawKey)).ToArray();

            foreach (var group in Records(csv).GroupBy(record => GetInt(record, WeatherFieldKey.NUM_POSTE))) {
                var firstRow = group.First();
                var availableKeys = new List<WeatherFieldKey>();
                for (int i = 0; i < firstRow.values.Count(); i++)
                {
                    if(!string.IsNullOrEmpty(firstRow.values[i]))
                    {
                        availableKeys.Add(this.availableKeys[i]);
                    }
                }
                posts.Add(group.Key, new WeatherPostDataset(this, group.Key, Get(firstRow, WeatherFieldKey.NOM_USUEL), availableKeys));
            };
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

        public IEnumerable<WeatherRecord> Records(int postID)
        {
            return Records(new CSVLoader(WeatherFileName(department, year), postID));
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