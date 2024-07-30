using Assets.Resources.Models;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Activities
{
    public class Activity
    {
        public string name;
        public ActivityModel model;

        public int minSuitability;
        public Tuple<int, int> tempRange, windRange, cloudRange, rainRange;

        public Activity(string name, float[] tempRange, float[] windRange, float[] rainRange, float[] cloudRange)
        {
            this.name = name;
            this.model = new ActivityModel(tempRange, windRange, rainRange, cloudRange);
        }

        public int Suitability(WeatherCondition weather)
        {
            var res = (int)model.Predict(weather);
            Debug.Log($"Prediction for weather condition {weather.Temperature}°, {weather.WindSpeed}kmh windspeed, {weather.Rainfall} rain, {weather.Cloudiness}% cloudiness: {res}%");
            return res;
        }
    }
}