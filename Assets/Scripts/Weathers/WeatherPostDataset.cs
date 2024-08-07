using Assets.Resources.Activities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherPostDataset
    {
        public string post;
        public List<WeatherRecord> records;

        public WeatherPostDataset(string post, List<WeatherRecord> records)
        {
            this.post = post;
            this.records = records;
        }
    }
}