using Assets.Resources.Activities;
using Assets.Scripts;
using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Foenn.ETL.CSV
{
    public class WeatherHistoryLoader
    {
        public int id;

        public const string WEATHER_PATH = "Weathers/";

        public List<WeatherPostDataset> posts = new List<WeatherPostDataset>();

        public int year;

        public WeatherHistoryLoader(string fileName, int year, int department, string fileText, List<Activity> activities)
        {
            this.year = year;
            this.id = department;
            //Debug.Log($"Load CSV {fileName}");
            var csv = new CSVLoader().Extract(fileText, year);
            //Debug.Log($"Done");
            foreach (var group in csv.lines.GroupBy(record => record.Get(AttributeKey.NOM_USUEL)))
            {
                posts.Add(new WeatherPostDataset(group.Key, department, group));
            }
        }

        public static string ExtractValue(string input)
        {
            // D�finir une expression r�guli�re pour trouver un nombre entre "H_" et "_latest"
            string pattern = @"H_(\d+)_latest";

            // Utiliser Regex pour correspondre � l'expression r�guli�re dans la cha�ne d'entr�e
            Match match = Regex.Match(input, pattern);

            // Si une correspondance est trouv�e, retourner le groupe captur� (le nombre)
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // Si aucune correspondance n'est trouv�e, retourner null ou une valeur par d�faut
            return null;
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