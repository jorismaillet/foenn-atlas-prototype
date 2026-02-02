using Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public class WeatherCategory
    {
        public string name;
        public List<MetricGroupCondition> conditions;

        public WeatherCategory(string name, params MetricGroupCondition[] conditions)
        {
            this.name = name;
            this.conditions = conditions.ToList();
        }
    }
}