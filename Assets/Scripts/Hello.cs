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
        var city = "Plomelin";
        var department = 29;
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

        //TODO FF et FF2 sont des valeurs similaires, il faudrait pouvoir les regrouper pour étendre les villes compatibles
        //TODO Ajouter ses heures d'activité
        var activity = new Activity("Kayak", 10, new TimeCondition(15, 19), new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.T, 24, 30),
            new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
            new WeatherFieldCondition(WeatherFieldKey.FF2, 0, 5) // Certaines valeurs ne sont pas disponibles dans les posts
        }); // TODO RecordType with default range

        if(activity.conditions.Any(condition =>
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

        var suitHours = postWeather.records.Where(records => activity.Suits(records));
        var suitDays = suitHours.GroupBy(record => Day(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
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
}
