using Assets.Scripts.Foenn.Engine.Weathers;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions
{
    public class WeatherFieldCondition : IActivityCondition {
        public List<WeatherRecordFieldKey> keys;
        public float min;
        public float max;
        public ConditionImportanceKey importance;

        public WeatherFieldCondition(WeatherRecordFieldKey key, float min, float max)
        {
            this.keys = new List<WeatherRecordFieldKey>() { key };
            this.min = min;
            this.max = max;
        }
        public WeatherFieldCondition(List<WeatherRecordFieldKey> keys, float min, float max)
        {
            this.keys = keys;
            this.min = min;
            this.max = max;
        }

        public bool Match(WeatherRecord record)
        {
            return keys.Intersect(record.values.Keys).Any(key =>
            {
                var value = record.GetFloat(key);
                return min <= value && max >= value;
            });
        }
    }
}