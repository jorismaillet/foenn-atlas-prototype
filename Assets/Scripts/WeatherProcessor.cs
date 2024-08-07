using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using Assets.Scripts;
using Assets.Scripts.Activities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WeatherProcessor
{
    private int year;
    private int department;
    private List<Activity> activities;
    private string fileText;

    public WeatherProcessor(int year, int department, List<Activity> activities, string fileText)
    {
        this.year = year;
        this.department = department;
        this.activities = activities;
        this.fileText = fileText;
    }

    public async Task Process(Action<DepartmentRanking> Callback)
    {
        var keysToLoad = new List<WeatherFieldKey>() { WeatherFieldKey.NOM_USUEL, WeatherFieldKey.AAAAMMJJHH }.Union(activities.SelectMany(a => a.Keys())).Distinct().ToList();
        var dataset = new WeatherDataset(year, department, fileText, activities, keysToLoad);
        var res = await ProcessDepartmentRanking(dataset.posts, dataset);
        Callback(res);
    }

    private async Task<DepartmentRanking> ProcessDepartmentRanking(List<WeatherPostDataset> posts, WeatherDataset dataset)
    {
        var allDepartmentRanking = new List<Tuple<string, int>>();
        await Task.WhenAll(posts.Select(post => Task.Run(() =>
                allDepartmentRanking.Add(ProcessPostRanking(post, dataset))
        )).ToArray());
        Debug.Log($"Ranking for {department}:");
        var ranking = allDepartmentRanking.OrderByDescending(tuple => tuple.Item2).Take(3);
        foreach (var res in ranking)
        {
            Debug.Log($"{res.Item1}: {res.Item2} days");
        }
        return new DepartmentRanking(department, ranking.ToList());
    }


    private Tuple<string, int> ProcessPostRanking(WeatherPostDataset post, WeatherDataset dataset)
    {
        return Tuple.Create(post.post, StatsFor(dataset, post));
    }

    private void GetPostFromCity()
    {
        /*
           Debug.Log($"Finding post match for city {city}");
        var post = weather.Post(city);
        if(post == null)
        {
            Debug.Log("Didn't find any post for this city");
            return 0;
        }
        Debug.Log($"Found {post}");
        */
    }

    private void ProcessDetailedCityStatistics()
    {
        //Debug.Log($"The post {post} has {postWeather.records.Count()} entries and the average temperature recorded in {year} is {postWeather.AverageTemperature()}");
        /*var suitWeeks = suitHours.GroupBy(record => WeekOfYear(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
      var suitMonths = suitHours.GroupBy(record => Month(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
      var match = suitHours.Count();
      var total = postWeather.records.Count;
      Debug.Log($"Results for {activity.name} around {post}:");
      Debug.Log($"Activity suits {((float)match / total) * 100.0F}% of the time ({match} hours of suitability, in {suitDays.Count()} days");
      if (suitDays.Count > 0) {
          Debug.Log($"{ match / suitDays.Count()} hours per available day in average)");
      }*/
        /*Debug.Log($"Activity suits in {suitWeeks.Count()} weeks (expected at least {activity.minWeekFrequencyPerYear} weeks)");
        Debug.Log($"Activity is available in months: {string.Join(", ", suitMonths.Select(m => m.Key))}");
        foreach (var monthGroup in suitMonths)
        {
            var daysInMonth = monthGroup.GroupBy(record => Day(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
            Debug.Log($"{monthGroup.Key}: {daysInMonth.Count()} days");
        }

        if (suitWeeks.Count() >= activity.minWeekFrequencyPerYear)
        {
            Debug.Log($"Activity suits in {post}");
        }
        else
        {
            Debug.Log($"Activity does not suit in {post}");
        }*/
    }

    public int StatsFor(WeatherDataset dataset, WeatherPostDataset weatherPost)
    {
        Debug.Log($"{dataset.id} - {weatherPost.post}");

        var suit = activities.Select(activity => new ActivitySuit(activity, weatherPost.records));

        var res = suit.SelectMany(a => a.suitDays).Distinct().Count();

        //Debug.Log($"{weatherPost.post}: {res}");
        return res;
    }
}
