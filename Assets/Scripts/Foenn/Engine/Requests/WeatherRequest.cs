using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.ETL.CSV;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Foenn.Engine.Requests
{
    public class WeatherRequest
    {

        private void DataFor(int weekOfYear, WeatherPostDataset post)
        {
            var records = post.records;
            foreach (var record in records.Where(r => TimeUtils.WeekOfYear(r.Get(AttributeKey.AAAAMMJJHH)) == weekOfYear))
            {
                Debug.Log(string.Join(" - ", record.values.Select(keyvalue => $"{keyvalue.Key}: {keyvalue.Value}").ToArray()));
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

        private WeatherPostDataset GetPostFromCity(string city, int department, List<Activity> activities)
        {
            var keysToLoad = new List<AttributeKey>() { AttributeKey.NOM_USUEL, AttributeKey.AAAAMMJJHH }.Union(activities.SelectMany(a => a.Keys())).Distinct().ToList();
            var textAsset = Resources.Load<TextAsset>(WeatherHistoryLoader.WeatherFileName(department, year));
            var fileText = textAsset.text;
            return new WeatherPostDataset(city, year, fileText, keysToLoad);

        }

        private void FinalRanking()
        {
            Debug.Log("Final departments ranking:");

            var top = result.SelectMany(dep => dep.Value.ranking).OrderByDescending(rank => rank.rank).Take(3);
            foreach (var rank in top)
            {
                Debug.Log($"{rank.post.department}: {rank.rank} ({rank.post.post})");
            }
            finished = true;

            var best = top.First();
            var bestcity = best.post.post;
            var bestdep = best.post.department;
            Debug.Log($"Display best heatmap ({best.post.post})");
            Heatmap(bestcity, bestdep);
        }

        private Task StatsFor(int department)
        {



            var fileName = WeatherHistoryLoader.WeatherFileName(department, year);
            //Debug.Log($"Load resource {fileName}");
            var textAsset = Resources.Load<TextAsset>(fileName);
            var fileText = textAsset.text;
            Debug.Log($"Start departement {department}");
            /*_ = new WeatherProcessor(fileName, year, department, activities, fileText).Process((res) =>
            {
                result.Add(department, res);
                inprogress.Clear(); Resources.UnloadAsset(textAsset);
            });*/
            return new Task(() =>
            {
                _ = RunQuery(department, year);
                /*_ = new WeatherProcessor(fileName, year, department, activities, fileText).Process((res) =>
                {
                    result.Add(department, res);
                    inprogress.Clear(); Resources.UnloadAsset(textAsset);
                });*/
            });
        }
    }
}