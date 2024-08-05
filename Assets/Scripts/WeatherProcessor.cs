using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using Assets.Scripts;
using Assets.Scripts.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WeatherProcessor
{
    private int department = 29;
    private int year = 2023;
    private Activity activity;
    private WeatherDataset weather;

    private List<WeatherPostDataset> postsForActivity;
    private DepartmentRanking ranking;

    public WeatherProcessor()
    {
        BuildActity();
    }

    public async Task Process()
    {
        await InitializeDepartment();
        InitializePostsForActivity();
        await ProcessDepartmentRanking();
    }

    private async Task<DepartmentRanking> ProcessDepartmentRanking()
    {
        var allDepartmentRanking = new List<Tuple<string, int>>();
        foreach (var post in postsForActivity)
        {
            var postRank = await ProcessPostRanking(post);
            allDepartmentRanking.Add(postRank);
        }
        var ranking = allDepartmentRanking.OrderByDescending(tuple => tuple.Item2).Take(3);
        Debug.Log($"Best place for {activity.name} in department {department} in {year} is {ranking.First().Item1} with {ranking.First().Item2} days of suitability");
        return new DepartmentRanking(department, ranking.ToList());
    }

    private async Task<Tuple<string, int>> ProcessPostRanking(WeatherPostDataset post)
    {
        Debug.Log($"Stats for {post.post}");
        return Tuple.Create(post.post, await StatsFor(post, activity));
    }

    private void InitializePostsForActivity()
    {
        postsForActivity = weather.posts.Values.Where(post => post.HasRecordsFor(activity)).ToList();
        Debug.Log($"{postsForActivity.Count}/{weather.posts.Count} posts have enough data to check this activity");
    }

    private async Task InitializeDepartment()
    {
        Debug.Log($"Loading file {WeatherDataset.WeatherFileName(department, year)}");
        weather = await WeatherDataset.Load(department, year); //TODO Load necessary columns only when doing stats on all posts + early skip posts without the info
                                                         //Debug.Log($"There are {weather.EntriesQuantity()} entries for {weather.PostsQuantity()} posts in {year} for department {department}");
    }

    private void BuildActity()
    {
        // Certaines valeurs ne sont pas disponibles dans les posts
        //TODO FF et FF2 sont des valeurs similaires, il faudrait pouvoir les regrouper pour étendre les villes compatibles
        //TODO Ajouter ses heures d'activité
        //TODO pour les jours non compatibles, indiquer les raisons (ex: trop de vent)
        activity = new Activity("Kayak", 10, 3, new TimeCondition(14, 20), new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.T, 24, 30),
            new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
            new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2 }, 0, 5) // Certaines valeurs ne sont pas disponibles dans les posts
        }, new List<WeatherFieldCondition>());
        /*activity = new Activity("Tennis", 0, 3, new TimeCondition(9, 20), new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 5, 30),
            new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 2)
        }, new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0)
        }); // TODO RecordType with default range*/
        /*activity = new Activity("Piscine", 0, 2, new TimeCondition(9, 22), new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 25, 35),
            new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 4)/*,
            new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.N, WeatherFieldKey.N1, WeatherFieldKey.NBAS }, 0, 3)
        }, new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0)
        });*/
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

    public async Task<int> StatsFor(WeatherPostDataset weatherPost, Activity activity)
    {
        Debug.Log($"Loading records for post {weatherPost.post}");
        await weatherPost.Load(department, year);

        List<WeatherRecord> suitHours = new List<WeatherRecord>();
        List<string> suitDays = new List<string>();

        suitHours = weatherPost.records.Where(records => activity.SuitsHour(records, weatherPost.availableKeys)).ToList();
        suitDays = suitHours.GroupBy(record => TimeHelper.Day(record.Get(WeatherFieldKey.AAAAMMJJHH))).ToList().Where(group =>
        {
            var hoursForDay = group.ToList();
            var suit = activity.SuitsDay(hoursForDay, weatherPost.availableKeys);
            if (suit)
            {
                //Check continous hours
                var maxConsecutiveHours = TimeHelper.MaxConsecutiveHours(hoursForDay.Select(record => TimeHelper.Hour(record.Get(WeatherFieldKey.AAAAMMJJHH))).ToList());
                if (maxConsecutiveHours < activity.minContinousHours)
                {
                    suit = false;
                }
            }
            if (!suit)
            {
                suitHours = suitHours.Except(hoursForDay).ToList();
            }
            return suit;
        }).Select(g => g.Key).ToList();

        var res = suitDays.Count();
        Debug.Log($"{weatherPost.post}: {res}");
        return res;
    }
}
