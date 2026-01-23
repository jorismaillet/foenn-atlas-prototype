using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Models.WeatherRecords {
    public class CityWeatherRecord {
        public City city;
        public int year;
        public List<WeatherRecord> records;
    }
}