using Assets.Resources.Weathers;
using Assets.Scripts.Activities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Assets.Resources.Activities
{
    public class Activity
    {
        public string name;
        public TimeCondition timeCondition;
        public int minWeekFrequencyPerYear;
        public List<WeatherFieldCondition> conditions;

        public Activity(string name, int minWeekFrequencyPerYear, TimeCondition timeCondition, List<WeatherFieldCondition> conditions)
        {
            this.name = name;
            this.minWeekFrequencyPerYear = minWeekFrequencyPerYear;
            this.timeCondition = timeCondition;
            this.conditions = conditions;
        }

        private bool SuitsHour(string AAAAMMJJHH)
        {
            if (timeCondition == null)
            {
                return true;
            }
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime.Hour >= timeCondition.minHour && dateTime.Hour <= timeCondition.maxHour;
        }

        public bool Suits(WeatherRecord record)
        {
            return SuitsHour(WeatherDataset.Instance.Get(record, WeatherFieldKey.AAAAMMJJHH)) && conditions.All(condition => condition.Match(record));
        }
    }
}