using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Models
{
    public class WeatherPreferences
    {
        public float importance; // 0 - 1
        public float min;
        public float max;
        public float ideal; // within min and max
        public float tolerance; // 0 - 1
    }
}