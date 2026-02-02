using Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Engine.Metrics;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities {
    public class Activity {
        public string name;

        public int minContinousHours;
        public int minWeekFrequencyPerYear;
        public List<IActivityCondition> conditions;

        public List<Dictionary<MetricKey, float>> rankedWeathers;

        public Activity(string name, params IActivityCondition[] conditions) {
            this.name = name;
            this.conditions = new List<IActivityCondition>(conditions);
        }
    }
}
