using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Assets.Scripts.Activities
{
    public class ActivitySuit
    {
        public Activity activity;
        public List<WeatherRecord> suitHours;
        public List<string> suitDays;

        public ActivitySuit(Activity activity, List<WeatherRecord> records)
        {
            this.activity = activity;
            suitHours = records.Where(record => activity.SuitsHour(record)).ToList();
            suitDays = suitHours.GroupBy(record => TimeHelper.Day(record.Get(WeatherFieldKey.AAAAMMJJHH))).ToList().Where(group =>
            {
                var hoursForDay = group.ToList();
                var suit = activity.SuitsDay(hoursForDay);
                if (suit)
                {
                    //Check continous hours
                    var maxConsecutiveHours = TimeHelper.MaxConsecutiveHours(hoursForDay.Select(record => TimeHelper.Hour(record.Get(WeatherFieldKey.AAAAMMJJHH))).ToList());
                    if (maxConsecutiveHours < activity.minContinousHours)
                    {
                        suit = false;
                    }
                }
                if (!suit)
                {
                    suitHours = suitHours.Except(hoursForDay).ToList();
                }
                return suit;
            }).Select(g => g.Key).ToList();
        }
    }
}