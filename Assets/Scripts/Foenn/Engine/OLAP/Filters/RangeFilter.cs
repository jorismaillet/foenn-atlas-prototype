using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class RangeFilter : Filter
    {
        public string attributeName;
        public int minValue, maxValue;

        public RangeFilter(string attributeName, int minValue, int maxValue) : base() {
            this.attributeName = attributeName;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}