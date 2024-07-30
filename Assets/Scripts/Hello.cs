using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class Hello : MonoBehaviour
{
    void Start()
    {
        var post = "PLOMELIN-INRAE";
        var department = 29;
        var year = 2023;

        //Debug.Log($"Loading file {WeatherDataset.WeatherFileName(department, year)}");
        var weather = WeatherDataset.Load(department, year);
        //Debug.Log($"There are {weather.EntriesQuantity()} entries for {weather.PostsQuantity()} posts in {year} for department {department}");

        //Debug.Log($"Loading records for post {post}");
        var postWeather = new WeatherPostDataset(weather, post);
        //Debug.Log($"The post {post} has {postWeather.records.Count()} entries and the average temperature recorded in {year} is {postWeather.AverageTemperature()}");

        var activity = new Activity("Kayak", new List<WeatherFieldCondition>()
        {
            new WeatherFieldCondition(WeatherFieldKey.T, 24, 32),
            new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
            new WeatherFieldCondition(WeatherFieldKey.FF2, 0, 6) // Certaines valeurs ne sont pas disponibles dans les posts
        }); // TODO RecordType with default range
        var suitHours = postWeather.records.Where(records => activity.Suits(records));
        var suitDays = suitHours.GroupBy(record => Day(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)));
        foreach(var dayGroup in suitDays)
        {
            Debug.Log($"{dayGroup.Key}: {dayGroup.Count()} hours");
        }
        var match = suitHours.Count();
        var total = postWeather.records.Count;
        Debug.Log($"Results for {activity.name} around {post} in {year}:");
        Debug.Log($"Activity suits {((float)match / total) * 100.0F}% of the time ({match} hours of suitability, in {suitDays.Count()} days ({match / suitDays.Count()} hours per available day in average)");
        Debug.Log($"Activity suits {((float)match / total) * 100.0F}%");
    }

    private string Day(string AAAAMMJJHH)
    {
        string format = "yyyyMMddHH";
        DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
        return dateTime.ToString("dd MMMM", new CultureInfo("fr-FR"));
    }
}
