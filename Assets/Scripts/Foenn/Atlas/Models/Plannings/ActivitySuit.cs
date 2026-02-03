using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Engine.OLAP;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings {
    public class ActivitySuit {
        public Activity activity;
        public List<string> suitDays;

        public ActivitySuit(Activity activity, IEnumerable<Row> records)
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
