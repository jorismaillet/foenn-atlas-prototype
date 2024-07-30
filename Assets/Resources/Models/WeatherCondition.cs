using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Activities
{
    public class WeatherCondition
    {
        public float Temperature { get; set; } // En degrés Celsius
        public float WindSpeed { get; set; } // En km/h
        public float Rainfall { get; set; } // En mm
        public float Cloudiness { get; set; } // En pourcentage

        public WeatherCondition(float temperature, float windSpeed, float rainfall, float cloudiness)
        {
            Temperature = temperature;
            WindSpeed = windSpeed;
            Rainfall = rainfall;
            Cloudiness = cloudiness;
        }
    }
}