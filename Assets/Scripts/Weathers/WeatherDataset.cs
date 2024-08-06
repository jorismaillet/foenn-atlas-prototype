using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherDataset
    {
        public int id;

        public const string WEATHER_PATH = "Weathers/";

        public List<WeatherFieldKey> datasetKeys;
        public Dictionary<int, WeatherPostDataset> posts = new Dictionary<int, WeatherPostDataset>();

        public int year;

        public WeatherDataset(int year, int department, CSVResult csv)
        {
            this.year = year;
            this.id = department;
            datasetKeys = csv.headers.Select(rawKey => Enum.Parse<WeatherFieldKey>(rawKey)).ToList();
            var records = Records(csv.lines);
            foreach (var group in records.GroupBy(record => record.GetInt(WeatherFieldKey.NUM_POSTE)))
            {
                var firstRow = group.First();
                var availableKeys = firstRow.values.Where(g => !string.IsNullOrEmpty(g.Value)).Select(g => g.Key).ToList();
                posts.Add(group.Key, new WeatherPostDataset(group.Key, firstRow.Get(WeatherFieldKey.NOM_USUEL), group.ToList(), availableKeys));
            };
        }

        public static WeatherDataset Load(string fileText, int year, int department)
        {
            var csv = new CSVLoader().LoadCSV(fileText); // Read all lines, add records in dataset posts
            var result = new WeatherDataset(year, department, csv);
            return result;
        }

        public WeatherPostDataset Post(string city)
        {
            return posts.Values.FirstOrDefault(post => post.post.IndexOf(city, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public static string WeatherFileName(int department, int year)
        {
            return $"{WEATHER_PATH}{FileName(department, year)}";
        }

        private IEnumerable<WeatherRecord> Records(IEnumerable<string> remainingLines)
        {
            var records = remainingLines.Select(line => new WeatherRecord(datasetKeys, line));
            var filteredRecords = records.Where(record => record.Get(WeatherFieldKey.AAAAMMJJHH).StartsWith(year.ToString()));
            return filteredRecords;
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