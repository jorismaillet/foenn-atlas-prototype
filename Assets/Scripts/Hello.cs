using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using Assets.Scripts.Activities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Hello : MonoBehaviour
{
    private int department = 29;
    private int year = 2023;
    private Activity activity;
    private WeatherDataset weather;

    private int cityIndex = 0;
    private List<WeatherPostDataset> postsForActivity;
    private List<Tuple<string, int>> result = new List<Tuple<string, int>>();

    //https://openweathermap.org/city/2997943 Pour plus de données
    private void Start()
    {
        //Debug.Log($"Loading file {WeatherDataset.WeatherFileName(department, year)}");
        weather = WeatherDataset.Load(department, year); //TODO Load necessary columns only when doing stats on all posts + early skip posts without the info
                                                         //Debug.Log($"There are {weather.EntriesQuantity()} entries for {weather.PostsQuantity()} posts in {year} for department {department}");

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


        postsForActivity = weather.posts.Values.Where(post => post.HasRecordsFor(activity)).ToList();
        Debug.Log($"{postsForActivity.Count}/{weather.posts.Count} has enough data to check this activity");

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

    void Update()
    {
        if (postsForActivity == null || cityIndex > postsForActivity.Count())
        {
            return;
        }
        else if (cityIndex == postsForActivity.Count())
        {
            var ranking = result.OrderByDescending(tuple => tuple.Item2);
            Debug.Log($"Best place for {activity.name} in department {department} in {year} is {ranking.First().Item1} with {ranking.First().Item2} days of suitability");
            cityIndex++;
            return;
        }
        var post = postsForActivity[cityIndex];
        Debug.Log($"Stats for {post.post}");
        result.Add(Tuple.Create(post.post, StatsFor(post, activity)));

        cityIndex++;
    }

    public int StatsFor(WeatherPostDataset weatherPost, Activity activity)
    {
        //Debug.Log($"Loading records for post {post}");
        var records = weatherPost.Records();
        //Debug.Log($"The post {post} has {postWeather.records.Count()} entries and the average temperature recorded in {year} is {postWeather.AverageTemperature()}");

        /* if (activity.hourlyConditions.Union(activity.cumulatedHourConditions).Any(condition =>
         {
             var record = postWeather.records.First();
             var value = weather.Get(record, condition.key);
             if (value == null || value == "")
             {
                 Debug.Log($"Error: {condition.key} is not available for post {post}");
                 return true;
             }
             if (!float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out float result))
             {
                 Debug.Log($"Error: {condition.key} expected float. value is {value}");
                 return true;
             }
             return false;
         })) {
             return 0;
         }*/

        List<WeatherRecord> suitHours = new List<WeatherRecord>();
        List<string> suitDays = new List<string>();

        suitHours = records.Where(records => activity.SuitsHour(records, weatherPost.availableKeys)).ToList();
        suitDays = suitHours.GroupBy(record => Day(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH))).ToList().Where(group =>
        {
            var hoursForDay = group.ToList();
            var suit = activity.SuitsDay(hoursForDay, weatherPost.availableKeys);
            if (suit)
            {
                //Check continous hours
                var maxConsecutiveHours = MaxConsecutiveHours(hoursForDay.Select(record => Hour(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH))).ToList());
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
        return suitDays.Count();
    }

    private int Hour(string AAAAMMJJHH)
    {
        string format = "yyyyMMddHH";
        DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
        return dateTime.Hour;
    }

    private string Day(string AAAAMMJJHH)
    {
        string format = "yyyyMMddHH";
        DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
        return dateTime.ToString("dd MMMM", new CultureInfo("fr-FR"));
    }

    private int WeekOfYear(string AAAAMMJJHH)
    {
        string format = "yyyyMMddHH";
        DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
        return GetIso8601WeekOfYear(dateTime);
    }

    private string Month(string AAAAMMJJHH)
    {
        string format = "yyyyMMddHH";
        DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
        return dateTime.ToString("MMMM", new CultureInfo("fr-FR"));
    }

    // This presumes that weeks start with Monday.
    // Week 1 is the 1st week of the year with a Thursday in it.
    public static int GetIso8601WeekOfYear(DateTime time)
    {
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            time = time.AddDays(3);
        }

        // Return the week of our adjusted day
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    public static int MaxConsecutiveHours(List<int> hours)
    {
        if (hours == null || hours.Count == 0)
        {
            return 0;
        }

        hours.Sort();
        int maxConsecutive = 1;
        int currentStreak = 1;

        for (int i = 1; i < hours.Count; i++)
        {
            if (hours[i] == hours[i - 1] + 1)
            {
                currentStreak++;
            }
            else
            {
                if (currentStreak > maxConsecutive)
                {
                    maxConsecutive = currentStreak;
                }
                currentStreak = 1;
            }
        }

        return Math.Max(maxConsecutive, currentStreak);
    }
}
