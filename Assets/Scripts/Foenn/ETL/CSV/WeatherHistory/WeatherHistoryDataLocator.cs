using System;

namespace Assets.Scripts.Foenn.ETL.CSV
{
    public class WeatherHistoryDataLocator
    {
        public const string WEATHER_PATH = "Weathers/";

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