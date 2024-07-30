using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Weathers
{
    public class WeatherPostDataset
    {
        private WeatherDataset dataset;
        public string post;
        public List<WeatherRecord> records;

        public WeatherPostDataset(WeatherDataset dataset, string post)
        {
            this.dataset = dataset;
            this.post = post;
            this.records = dataset.Records(post).ToList();
        }

        public int AverageTemperature()
        {
            return (int)(records.Select(entry => GetFloat(entry, WeatherFieldKey.T)).Average());
        }

        private float GetFloat(WeatherRecord record, WeatherFieldKey key)
        {
            return record.GetFloat(Array.IndexOf(dataset.keys, key));
        }
    }
}