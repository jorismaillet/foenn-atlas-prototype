using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Visualisations;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Inputs;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine
{
public class WeatherTaskExecutor : MonoBehaviour
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

    //TODO indiquer une ville sp�cifique et savoir son rang parmis tout le ranking

    private bool all = false;

    //https://openweathermap.org/city/2997943 Pour plus de donn�es
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
    }
}