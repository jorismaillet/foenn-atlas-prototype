using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using Assets.Scripts;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;

public class Main : MonoBehaviour
{
    private List<int> todo = new List<int>() {}, inprogress = new List<int>();
    private bool finished = false;

    public int year = 2023;

    private Dictionary<int, DepartmentRanking> result = new Dictionary<int, DepartmentRanking>();

    //TODO indiquer une ville spécifique et savoir son rang parmis tout le ranking

    bool all = true;

    //https://openweathermap.org/city/2997943 Pour plus de données
    private void Start()
    {
        var departmentFiles = CSVLoader.ListCsvFilesInResources(WeatherDataset.WEATHER_PATH).Select(f => ExtractValue(f)).Where(f => f != null).Select(f => int.Parse(f)).ToList();
        
        if(all)
        {
            todo.AddRange(departmentFiles);
        }
        else
        {
            todo.AddRange(new List<int>() { 29, 34 });
        }

        Debug.Log($"{departmentFiles.Count()} departments");

        foreach (var department in todo)
        {
            Debug.Log($"Loading {department}");
            var fileText = Resources.Load<TextAsset>(WeatherDataset.WeatherFileName(department, year)).text;
            new Task(() =>
            {
                var res = new WeatherProcessor(year, department, new List<Activity> { 
                /*Activity.Jardin, */Activity.Piscine, Activity.Kayak, Activity.Plage, Activity.Velo,
                Activity.Tennis, /*Activity.Ville, Activity.Randonnee,*/ Activity.DinerExterieur,
            }, fileText).Process();
                result.Add(department, res);
                inprogress.Clear();
            }).Start();
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
        if(result.Count.Equals(todo.Count))
        {
            FinalRanking();
        }

        /*
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
                StatsFor(next);
            }
        }*/
    }

    private void FinalRanking()
    {
        Debug.Log("Final departments ranking:");
        foreach (var dep in result.OrderByDescending(dep => dep.Value.ranking.First()).Take(3))
        {
            Debug.Log($"{dep.Key}: {dep.Value.ranking.First().Item1} ({dep.Value.ranking.First().Item2})");
        }
        finished = true;
    }

    private void StatsFor(int department)
    {
        var fileText = Resources.Load<TextAsset>(WeatherDataset.WeatherFileName(department, year)).text;
        Debug.Log($"Stats for {department}");
        new Task(() =>
        {
            var res = new WeatherProcessor(year, department, new List<Activity> { 
                /*Activity.Jardin, */Activity.Piscine, Activity.Kayak, Activity.Plage, Activity.Velo,
                Activity.Tennis, /*Activity.Ville, Activity.Randonnee,*/ Activity.DinerExterieur,
            }, fileText).Process();
            result.Add(department, res);
            inprogress.Clear();
        }).Start();
    }
}
