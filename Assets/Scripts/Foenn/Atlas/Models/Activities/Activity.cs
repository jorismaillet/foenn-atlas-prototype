using Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Engine.Weathers;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities {
    public class Activity {
        public string name;

        public int minContinousHours;
        public int minWeekFrequencyPerYear;
        public List<IActivityCondition> conditions;

        public List<WeatherMeasure> rankedWeathers;

        public Location location;

        public Activity(string name) {
            this.name = name;
        }
    }
}
