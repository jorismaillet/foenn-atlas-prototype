using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherDataset
    {
        public static WeatherDataset Instance;

        public const string WEATHER_PATH = "Weathers/";

        public List<WeatherFieldKey> availableKeys;
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

        public static async Task<WeatherDataset> Load(int department, int year)
        {
            Instance = new WeatherDataset(department, year);
            await Instance.Load();
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
        }

        public async Task Load()
        {
            Debug.Log("Read CSV");
            var csv = new CSVLoader();
            var remainingLines = await csv.Load(WeatherFileName(department, year));
            Debug.Log("0");
            availableKeys = csv.header.Select(rawKey => Enum.Parse<WeatherFieldKey>(rawKey)).ToList();
            Debug.Log("1");
            var records = Records(remainingLines);
            Debug.Log("2");
            foreach (var group in records.GroupBy(record => record.GetInt(WeatherFieldKey.NUM_POSTE)))
            {
                var firstRow = group.First();
                var availableKeys = firstRow.values.Where(g => !string.IsNullOrEmpty(g.Value)).Select(g => g.Key).ToList();
                posts.Add(group.Key, new WeatherPostDataset(this, group.Key, firstRow.Get(WeatherFieldKey.NOM_USUEL), availableKeys));
            };
        }

        public static string WeatherFileName(int department, int year)
        {
            return $"{WEATHER_PATH}{FileName(department, year)}";
        }

        private IEnumerable<WeatherRecord> Records(IEnumerable<string> remainingLines)
        {
            Debug.Log("3");
            var records = remainingLines.Select(line => new WeatherRecord(availableKeys, line));
            var filteredRecords = records.Where(record => record.Get(WeatherFieldKey.AAAAMMJJHH).StartsWith(year.ToString()));
            Debug.Log("4");
            return filteredRecords;
        }
    }
}