using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Assets.Scripts.Legacy.Results
{
    public class ActivitySuit
    {
        public ActivitiesProcessor activity;
        public List<string> suitDays;

        public ActivitySuit(ActivitiesProcessor activity, IEnumerable<WeatherRecord> records)
        {
            this.activity = activity;
            var suitHours = records.Where(record => activity.SuitsHour(record));
            suitDays = suitHours.GroupBy(record => TimeUtils.Day(record.Get(WeatherRecordFieldKey.AAAAMMJJHH))).Where(hoursForDay =>
            {
                var suit = activity.SuitsDay(hoursForDay);
                if (suit)
                {
                    //Check continous hours
                    var maxConsecutiveHours = TimeUtils.MaxConsecutiveHours(hoursForDay.Select(record => TimeUtils.Hour(record.Get(WeatherRecordFieldKey.AAAAMMJJHH))).ToList());
                    if (maxConsecutiveHours < activity.minContinousHours)
                    {
                        suit = false;
                    }
                }
                return suit;
            }).Select(g => g.Key).ToList();
        }
    }
}