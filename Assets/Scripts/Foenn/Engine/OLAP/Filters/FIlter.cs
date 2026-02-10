using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;

namespace Assets.Scripts.Foenn.Engine.Filters
{
    public abstract class Filter
    {
        public WeatherHistoryAttributeKey filteredAttributeKey;

        protected Filter(WeatherHistoryAttributeKey filteredAttributeKey)
        {
            this.filteredAttributeKey = filteredAttributeKey;
        }
    }
}