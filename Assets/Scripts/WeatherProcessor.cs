using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeatherProcessor
{
    private int year;
    private int department;
    private Activity activity;
    private string fileText;

    public WeatherProcessor(int year, int department, Activity activity, string fileText)
    {
        this.year = year;
        this.department = department;
        this.activity = activity;
        this.fileText = fileText;
    }

    public void Process()
    {
        var dataset = WeatherDataset.Load(fileText, year, department);
        var posts = InitializePostsForActivity(dataset);
        ProcessDepartmentRanking(posts, dataset);

        //Debug.Log($"There are {weather.EntriesQuantity()} entries for {weather.PostsQuantity()} posts in {year} for department {department}");
        //TODO Load necessary columns only when doing stats on all posts + early skip posts without the info
    }

    private DepartmentRanking ProcessDepartmentRanking(List<WeatherPostDataset> posts, WeatherDataset dataset)
    {
        var allDepartmentRanking = new List<Tuple<string, int>>();
        foreach (var post in posts)
        {
            var postRank = ProcessPostRanking(post, dataset);
            allDepartmentRanking.Add(postRank);
        }
        var ranking = allDepartmentRanking.OrderByDescending(tuple => tuple.Item2).Take(3);
        Debug.Log($"Best place for {activity.name} in department {department} in {year} is {ranking.First().Item1} with {ranking.First().Item2} days of suitability");
        return new DepartmentRanking(department, ranking.ToList());
    }

    private Tuple<string, int> ProcessPostRanking(WeatherPostDataset post, WeatherDataset dataset)
    {
        return Tuple.Create(post.post, StatsFor(dataset, post, activity));
    }

    private List<WeatherPostDataset> InitializePostsForActivity(WeatherDataset dataset)
    {
        var result = dataset.posts.Values.Where(post => post.HasRecordsFor(activity)).ToList();
        Debug.Log($"{result.Count}/{dataset.posts.Count} posts have enough data to check this activity");
        return result;
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

    public int StatsFor(WeatherDataset dataset, WeatherPostDataset weatherPost, Activity activity)
    {
        weatherPost.Load(year, fileText, dataset.datasetKeys);

        List<WeatherRecord> suitHours = new List<WeatherRecord>();
        List<string> suitDays = new List<string>();

        suitHours = weatherPost.records.Where(records => activity.SuitsHour(records)).ToList();
        suitDays = suitHours.GroupBy(record => TimeHelper.Day(record.Get(WeatherFieldKey.AAAAMMJJHH))).ToList().Where(group =>
        {
            var hoursForDay = group.ToList();
            var suit = activity.SuitsDay(hoursForDay);
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
