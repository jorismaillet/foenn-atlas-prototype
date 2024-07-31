using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using Assets.Scripts.Activities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class Hello : MonoBehaviour
{
    void Start()
    {
        var city = "Montpellier";
        var department = 34;
        var year = 2023;

        //Debug.Log($"Loading file {WeatherDataset.WeatherFileName(department, year)}");
        var weather = WeatherDataset.Load(department, year);
        //Debug.Log($"There are {weather.EntriesQuantity()} entries for {weather.PostsQuantity()} posts in {year} for department {department}");

        Debug.Log($"Finding post match for city {city}");
        var post = weather.Post(city);
        if(post == null)
        {
            Debug.Log("Didn't find any post for this city");
            return;
        }
        Debug.Log($"Found {post}");

        //Debug.Log($"Loading records for post {post}");
        var postWeather = new WeatherPostDataset(weather, post);
        //Debug.Log($"The post {post} has {postWeather.records.Count()} entries and the average temperature recorded in {year} is {postWeather.AverageTemperature()}");

        Debug.Log($"Total rain in {city} in {year}: {postWeather.records.Sum(record => weather.GetFloat(record, WeatherFieldKey.RR1))}mm");


        // Certaines valeurs ne sont pas disponibles dans les posts
        //TODO FF et FF2 sont des valeurs similaires, il faudrait pouvoir les regrouper pour étendre les villes compatibles
        //TODO Ajouter ses heures d'activité
        /*var activity = new Activity("Kayak", 10, new TimeCondition(15, 19), new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.T, 24, 30),
            new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
            new WeatherFieldCondition(WeatherFieldKey.FF2, 0, 5) // Certaines valeurs ne sont pas disponibles dans les posts
        }); // TODO RecordType with default range*/
        var activity = new Activity("Tennis", 0, 3, new TimeCondition(9, 20), new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.T, 5, 30),
            new WeatherFieldCondition(WeatherFieldKey.FF, 0, 2) 
        }, new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0)
        }); // TODO RecordType with default range

        if (activity.hourlyConditions.Union(activity.cumulatedHourConditions).Any(condition =>
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
            return;
        }

        var suitHours = postWeather.records.Where(records => activity.SuitsHour(records)).ToList();
        var suitDays = suitHours.GroupBy(record => Day(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH))).ToList().Where(group =>
        {
            var hoursForDay = group.ToList();
            var suit = activity.SuitsDay(hoursForDay);
            if(suit)
            {
                //Check continous hours
                var maxConsecutiveHours = MaxConsecutiveHours(hoursForDay.Select(record => Hour(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH))).ToList());
                if (maxConsecutiveHours < activity.minContinousHours)
                {
                    suit = false;
                }
            }
            if(!suit)
            {
                suitHours = suitHours.Except(hoursForDay).ToList();
            }
            return suit;
        });
        var suitWeeks = suitHours.GroupBy(record => WeekOfYear(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
        var suitMonths = suitHours.GroupBy(record => Month(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
        var match = suitHours.Count();
        var total = postWeather.records.Count;
        Debug.Log($"Results for {activity.name} around {post} in {year}:");
        Debug.Log($"Activity suits {((float)match / total) * 100.0F}% of the time ({match} hours of suitability, in {suitDays.Count()} days ({match / suitDays.Count()} hours per available day in average)");
        Debug.Log($"Activity suits in {suitWeeks.Count()} weeks (expected at least {activity.minWeekFrequencyPerYear} weeks)");
        Debug.Log($"Activity is available in months: {string.Join(", ", suitMonths.Select(m => m.Key))}");
        foreach (var monthGroup in suitMonths)
        {
            var daysInMonth = monthGroup.GroupBy(record => Day(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
            Debug.Log($"{monthGroup.Key}: {daysInMonth.Count()} days");
        }

        if (suitWeeks.Count() >= activity.minWeekFrequencyPerYear)
        {
            Debug.Log($"Activity suits in {city}");
        }
        else
        {
            Debug.Log($"Activity does not suit in {city}");
        }
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
