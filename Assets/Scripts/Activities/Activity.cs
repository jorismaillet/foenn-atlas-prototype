using Assets.Resources.Weathers;
using Assets.Scripts.Activities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Assets.Resources.Activities
{
    public class Activity
    {
        public string name;
        public TimeCondition timeCondition;
        public int minContinousHours;
        public int minWeekFrequencyPerYear;
        public List<WeatherFieldCondition> hourlyConditions, cumulatedHourConditions;

        public static Activity Kayak = new Activity("Kayak", 10, 3, new TimeCondition(14, 20), 
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 24, 30),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2}, 0, 5) // Certaines valeurs ne sont pas disponibles dans les posts
            },
            new List<WeatherFieldCondition>() { 
            }
        );

        public static Activity Tennis = new Activity("Tennis", 0, 3, new TimeCondition(9, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 5, 30),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 2)
            },
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0)
            }
        ); // TODO RecordType with default range

        public static Activity Piscine = new Activity("Piscine", 0, 2, new TimeCondition(9, 22),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 25, 35),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 4),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.N, WeatherFieldKey.N1, WeatherFieldKey.NBAS }, 0, 3)
            },
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0)
            }
        );

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

        public bool Suits(WeatherRecord hourRecord, List<WeatherFieldCondition> conditions)
        {
            return conditions.All(condition => condition.Match(hourRecord));
        }

        public bool SuitsHour(WeatherRecord hourRecord)
        {
            return SuitsHour(hourRecord.Get(WeatherFieldKey.AAAAMMJJHH)) && Suits(hourRecord, hourlyConditions);
        }

        public bool SuitsDay(List<WeatherRecord> selectedHourRecordsForDay)
        {
            return selectedHourRecordsForDay.All(record => Suits(record, cumulatedHourConditions));
        }
    }
}