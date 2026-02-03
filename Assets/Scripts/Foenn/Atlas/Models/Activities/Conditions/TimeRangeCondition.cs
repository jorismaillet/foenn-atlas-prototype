using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.OLAP;
using System;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions
{
    public class TimeRangeCondition : IActivityCondition
    {
        public int minHour, maxHour;

        public TimeRangeCondition(int minHour, int maxHour)
        {
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
            return DateTime.Parse(row.AttributeValue(AttributeKey.AAAAMMJJHH).value);
        }
    }
}
