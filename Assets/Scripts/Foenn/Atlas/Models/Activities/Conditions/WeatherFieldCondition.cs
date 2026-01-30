using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Weathers;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions
{
    public class WeatherFieldCondition : IActivityCondition, Attribute {
        public List<AttributeKey> keys;
        public float min;
        public float max;
        public ConditionImportanceKey importance;

        public WeatherFieldCondition(AttributeKey key, float min, float max)
        {
            this.keys = new List<AttributeKey>() { key };
            this.min = min;
            this.max = max;
        }
        public WeatherFieldCondition(List<AttributeKey> keys, float min, float max)
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