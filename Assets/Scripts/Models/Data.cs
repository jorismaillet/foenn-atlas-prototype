using System.Collections.Generic;
using Assets.Scripts.Models.WeatherRecords;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Models {
    public class Data {
        public Dictionary<City, CityWeatherRecord> records = new Dictionary<City, CityWeatherRecord>();
    }
}