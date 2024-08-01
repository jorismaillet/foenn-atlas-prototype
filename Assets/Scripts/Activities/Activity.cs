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
        public int minContinousHours;
        public int minWeekFrequencyPerYear;
        public List<WeatherFieldCondition> hourlyConditions, cumulatedHourConditions;

        public Activity(string name, int minWeekFrequencyPerYear, int minContinuousHours, TimeCondition timeCondition, List<WeatherFieldCondition> hourlyConditions, List<WeatherFieldCondition> cumulatedHourConditions)
        {
            this.name = name;
            this.minWeekFrequencyPerYear = minWeekFrequencyPerYear;
            this.minContinousHours = minContinuousHours;
            this.timeCondition = timeCondition;
            this.hourlyConditions = hourlyConditions;
            this.cumulatedHourConditions = cumulatedHourConditions;
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

        public bool Suits(WeatherRecord hourRecord, List<WeatherFieldCondition> conditions, List<WeatherFieldKey> availableKeys)
        {
            return conditions.All(condition => condition.Match(hourRecord, availableKeys));
        }

        public bool SuitsHour(WeatherRecord hourRecord, List<WeatherFieldKey> availableKeys)
        {
            return SuitsHour(WeatherDataset.Instance.Get(hourRecord, WeatherFieldKey.AAAAMMJJHH)) && Suits(hourRecord, hourlyConditions, availableKeys);
        }

        public bool SuitsDay(List<WeatherRecord> selectedHourRecordsForDay, List<WeatherFieldKey> availableKeys)
        {
            return selectedHourRecordsForDay.All(record => Suits(record, cumulatedHourConditions, availableKeys));
        }
    }
}