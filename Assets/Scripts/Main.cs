using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using Assets.Scripts;
using Assets.Scripts.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEditor.Search;
using System.Data.SQLite;
using UnityEngine;
using System.IO;

public class Main : MonoBehaviour
{
    public Texture2D texture;

    private List<int> todo = new List<int>() {}, inprogress = new List<int>();
    private bool finished = false;

    private List<Activity> activities = new List<Activity> {
        //Activity.Moustiques1, Activity.Moustiques2, Activity.TropDePluie, Activity.Tempete, Activity.TropChaud,  Activity.TropDeVent,
        //Activity.Plage, Activity.DinerExterieur, Activity.Piscine, Activity.Kayak, Activity.Velo, Activity.Jardin, Activity.Randonnee, Activity.Tennis, Activity.Ville,
        Activity.JusteBien
            };
    private int year = 2023;

    private Dictionary<int, DepartmentRanking> result = new Dictionary<int, DepartmentRanking>();

    //TODO indiquer une ville spécifique et savoir son rang parmis tout le ranking

    private bool all = false;

    //https://openweathermap.org/city/2997943 Pour plus de données
    private void Start()
    {
        var departmentFiles = 
            CSVLoader.ListCsvFilesInResources(WeatherDataset.WEATHER_PATH)
            .Select(f => ExtractValue(f)).Where(f => f != null)
            .Select(f => int.Parse(f)).OrderBy(i => i).ToList();
        
        if(all)
        {
            todo.AddRange(departmentFiles);
        }
        else
        {
            todo.AddRange(new List<int>() { 75, 12, 48, 07 });
        }

        // Seed DB from CSV files (creates SQLite DB if needed)
        try
        {
            var dbPath = Path.Combine(Application.persistentDataPath, "departments.db");
            Debug.Log($"Seeding departments DB at: {dbPath}");
            DbSeeder.SeedDepartments(dbPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to seed departments DB: {ex}");
        }

        /*finished = true;
        Heatmap("Quimper", 29);*/

        //Debug.Log($"{departmentFiles.Count()} departments");
    }

    private void DataFor(int weekOfYear, WeatherPostDataset post)
    {
        var records = post.records;
        foreach (var record in records.Where(r => TimeHelper.WeekOfYear(r.Get(WeatherRecordFieldKey.AAAAMMJJHH)) == weekOfYear))
        {
            Debug.Log(string.Join(" - ", record.values.Select(keyvalue => $"{keyvalue.Key}: {keyvalue.Value}").ToArray()));
        }
    }

    private void Heatmap(string city, int department)
    {
        var post = GetPostFromCity(city, department, activities);
        var records = post.records.ToList();
        //DataFor(32, post);

        var points = activities.Sum(activity => post.records.Sum(r => activity.SuitsHour(r) ? activity.weight : 0));
        Debug.Log(points);
        var heatmap = records.Select(record => {
            var date = TimeHelper.Date(record.Get(WeatherRecordFieldKey.AAAAMMJJHH));

            var activity = activities.FirstOrDefault(a => a.SuitsHour(record));

            var res = new HourPoint(date, activity);
            return res;
        }).ToList();

        var image = GetComponent<UnityEngine.UI.Image>();
        var width = (int)image.GetComponent<RectTransform>().sizeDelta.x;
        var height = (int)image.GetComponent<RectTransform>().sizeDelta.y;
        texture = new Texture2D(width, height);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        foreach(var point in heatmap)
        {
            ColorPixel(texture, point.x, point.y, width, height, point.color);
        }
        image.sprite = sprite;
        texture.Apply();
    }

    private void ColorPixel(Texture2D texture, int x, int y, int width, int height, UnityEngine.Color color)
    {
        float scaleX = width / 365.0F;
        float scaleY = height / 24.0F;
        int minX = (int)(Mathf.Floor(x * scaleX));
        int maxX = (int)(Mathf.Ceil((x + 1) * scaleX));
        int minY = (int)(Mathf.Floor(y * scaleY));
        int maxY = (int)(Mathf.Ceil((y + 1) * scaleY));
        for (int XPixel = minX; XPixel < maxX; XPixel++)
        {
            for (int YPixel = minY; YPixel < maxY; YPixel++)
            {
                texture.SetPixel(XPixel, YPixel, color);
            }
        }
    }

    public static string ExtractValue(string input)
    {
        // Définir une expression régulière pour trouver un nombre entre "H_" et "_latest"
        string pattern = @"H_(\d+)_latest";

        // Utiliser Regex pour correspondre à l'expression régulière dans la chaîne d'entrée
        Match match = Regex.Match(input, pattern);

        // Si une correspondance est trouvée, retourner le groupe capturé (le nombre)
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // Si aucune correspondance n'est trouvée, retourner null ou une valeur par défaut
        return null;
    }

    private void Update()
    {
        if (finished) return;
        if (inprogress.Count == 0)
        {
            if (todo.Count == 0)
            {
                FinalRanking();
            }
            else
            {
                int next = todo[0];
                todo.RemoveAt(0);
                inprogress.Add(next);
                StatsFor(next).Start();
            }
        }
    }

    private WeatherPostDataset GetPostFromCity(string city, int department, List<Activity> activities)
    {
        var keysToLoad = new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.NOM_USUEL, WeatherRecordFieldKey.AAAAMMJJHH }.Union(activities.SelectMany(a => a.Keys())).Distinct().ToList();
        var textAsset = Resources.Load<TextAsset>(WeatherDataset.WeatherFileName(department, year));
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
        var bestcity = best.post.post ;
        var bestdep = best.post.department;
        Debug.Log($"Display best heatmap ({best.post.post})");
        Heatmap(bestcity, bestdep);
    }

    private Task StatsFor(int department)
    {
        var fileName = WeatherDataset.WeatherFileName(department, year);
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
            _ = new WeatherProcessor(fileName, year, department, activities, fileText).Process((res) =>
            {
                result.Add(department, res);
                inprogress.Clear(); Resources.UnloadAsset(textAsset);
            });
        });
    }
}
