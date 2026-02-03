using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.OLAP;
using System;
using System.Linq;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions {
    public class TimeRangeCondition : IActivityCondition
    {
        public int minHour, maxHour;

        public TimeRangeCondition(int minHour, int maxHour) {
            this.minHour = minHour;
            this.maxHour = maxHour;
        }

        public bool SuitsHour(Row row)
        {
            var date = Date(row);
            return date.Hour >= minHour && date.Hour <= maxHour;
        }

        private DateTime Date(Row row)
        {
            foreach (var dimension in row.dimensions)
            {
                foreach (var attributeValue in dimension.attributeValues)
                {
                    if (attributeValue.attribute.key.Equals(AttributeKey.AAAAMMJJHH))
                    {
                        return DateTime.Parse(attributeValue);
                    }
                }
            }
            throw new System.Exception("No AAAAMMJJHH attribute found in row dimensions.");
        }
    }
}
