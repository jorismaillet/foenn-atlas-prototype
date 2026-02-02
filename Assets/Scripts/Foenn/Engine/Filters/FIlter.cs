using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;

namespace Assets.Scripts.Foenn.Engine.Filters
{
    public abstract class Filter
    {
        public AttributeKey filteredAttributeKey;

        protected Filter(AttributeKey filteredAttributeKey)
        {
            this.filteredAttributeKey = filteredAttributeKey;
        }
    }
}