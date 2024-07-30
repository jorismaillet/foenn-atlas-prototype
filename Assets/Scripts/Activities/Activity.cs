using Assets.Resources.Weathers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Resources.Activities
{
    public class Activity
    {
        public string name;
        public List<WeatherFieldCondition> conditions;

        public Activity(string name, List<WeatherFieldCondition> conditions)
        {
            this.name = name;
            this.conditions = conditions;
        }

        public bool Suits(WeatherRecord record)
        {
            return conditions.All(condition => condition.Match(record));
        }
    }
}