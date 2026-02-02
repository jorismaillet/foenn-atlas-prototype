using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions;
using Assets.Scripts.Foenn.Engine.Weathers;
using Assets.Scripts.Foenn.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Assets.Scripts.Legacy.Processors
{
    public class ActivitiesProcessor
    {
        public string name;
        public int weight;
        public IActivityCondition condition;
        public int minContinousHours;
        public int minWeekFrequencyPerYear;
        public List<MetricGroupCondition> hourlyConditions, cumulatedHourConditions;
        public UnityEngine.Color color;

        private static UnityEngine.Color
            VertJardin = ColorUtils.Get(58, 251, 3),
            bleuCiel = ColorUtils.Get(3, 251, 248),
            bleuMer = ColorUtils.Get(15, 75, 227),
            sable = ColorUtils.Get(241, 255, 0),
            asphalteClaire = ColorUtils.Get(234, 234, 234),
            balleTennis = ColorUtils.Get(218, 255, 0),
            murs = ColorUtils.Get(144, 144, 144),
            vertForet = ColorUtils.Get(29, 122, 29),
            crepuscule = ColorUtils.Get(255, 183, 0),
            rouge = ColorUtils.Get(255, 0, 0);
        );

        


        public ActivitiesProcessor(string name, int weight, UnityEngine.Color color, int minWeekFrequencyPerYear, int minContinuousHours, IActivityCondition timeCondition, List<MetricGroupCondition> hourlyConditions, List<MetricGroupCondition> cumulatedHourConditions)
        {
            this.name = name;
            this.weight = weight;
            this.color = color;
            this.minWeekFrequencyPerYear = minWeekFrequencyPerYear;
            this.minContinousHours = minContinuousHours;
            this.condition = timeCondition;
            this.hourlyConditions = hourlyConditions;
            this.cumulatedHourConditions = cumulatedHourConditions;
        }

        private bool SuitsHour(string AAAAMMJJHH)
        {
            if (condition == null)
            {
                return true;
            }
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime.Hour >= condition.minHour && dateTime.Hour <= condition.maxHour;
        }

        public bool Suits(WeatherRecord hourRecord, List<MetricGroupCondition> conditions)
        {
            return conditions.All(condition => condition.Match(hourRecord));
        }

        public bool SuitsHour(WeatherRecord hourRecord)
        {
            return SuitsHour(hourRecord.Get(AttributeKey.AAAAMMJJHH)) && Suits(hourRecord, hourlyConditions);
        }

        public bool SuitsDay(IEnumerable<WeatherRecord> selectedHourRecordsForDay)
        {
            return selectedHourRecordsForDay.All(record => Suits(record, cumulatedHourConditions));
        }

        public IEnumerable<AttributeKey> Keys()
        {
            return hourlyConditions.SelectMany(c => c.keys).Union(cumulatedHourConditions.SelectMany(c => c.keys)).Distinct();
        }
    }
}