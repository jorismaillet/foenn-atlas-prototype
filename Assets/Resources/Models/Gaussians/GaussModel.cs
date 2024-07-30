using Assets.Resources.Activities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Models
{
    public class GaussModel
    {
        private WeatherPreferences temp, wind, rain, cloud;

        public GaussModel(WeatherPreferences temp, WeatherPreferences wind, WeatherPreferences rain, WeatherPreferences cloud)
        {
            this.temp = temp;
            this.wind = wind;
            this.rain = rain;
            this.cloud = cloud;
        }

        private WeatherCondition Normalize(WeatherCondition condition)
        {
            return new WeatherCondition(
                NormalizeValue(condition.Temperature, temp.min, temp.max),
                NormalizeValue(condition.WindSpeed, wind.min, wind.max),
                NormalizeValue(condition.Rainfall, rain.min, rain.max),
                NormalizeValue(condition.Cloudiness, cloud.min, cloud.max)
            );
        }

        private float NormalizeValue(float value, float min, float max)
        {
            if (min == max) return 0;
            return (value - min) / (max - min);
        }

        private float PredictGaussian(WeatherPreferences pref, float value)
        {
            var amplitude = pref.max - pref.min;
            var avg = (pref.max + pref.min) / 2;
            var leftDist = pref.ideal - pref.min;
            var rightDist = pref.max - pref.ideal;
            var maxDist = Math.Max(leftDist, rightDist);
            var distCoef = amplitude == 0 ? 0 : (maxDist * 2) / amplitude; // 1 - 2
            var normalizedTolerance = distCoef * (-pref.tolerance + 1 + (pref.tolerance * 1000)) / 1000; // 0.001 - 1
            if (pref.ideal == value) return 100;
            if (normalizedTolerance == 0 || amplitude == 0) return 0;
            return 100 * pref.importance * (float)(Math.Exp(Math.Pow(-(value - pref.ideal), 2)) / (normalizedTolerance * Math.Pow(amplitude, 2)));
        }

        private float PredictNormalized(WeatherCondition condition)
        {
            return ((PredictGaussian(temp, condition.Temperature) +
                    PredictGaussian(wind, condition.WindSpeed) +
                    PredictGaussian(rain, condition.Rainfall) +
                    PredictGaussian(cloud, condition.Cloudiness)) / 4) * 100;
        }

        public float Predict(WeatherCondition condition)
        {
            var normalizedCondition = Normalize(condition);
            if (OutOfRange(condition))
            {
                Debug.LogWarning("Condition is out of range");
                return 0;
            }
            return PredictNormalized(normalizedCondition);
        }

        private bool OutOfRange(WeatherCondition condition)
        {
            return
                OutOfRange(condition.Temperature, temp.min, temp.max) ||
                OutOfRange(condition.WindSpeed, wind.min, wind.max) ||
                OutOfRange(condition.Rainfall, rain.min, rain.max) ||
                OutOfRange(condition.Cloudiness, cloud.min, cloud.max);
        }

        private bool OutOfRange(float value, float min, float max)
        {
            return value < min || value > max;
        }
    }
}