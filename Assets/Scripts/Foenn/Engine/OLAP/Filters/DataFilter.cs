using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class DataFilter : Filter
    {
        public List<string> selectedValues;
        public DataFilterMode mode;

        public DataFilter(DataFilterMode mode, WeatherHistoryAttributeKey attributeKey, params string[] selectedValues) : base(attributeKey)
        {
            this.selectedValues = selectedValues.ToList();
            this.mode = mode;
        }
    }
}