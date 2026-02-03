using Assets.Scripts.Foenn.Atlas.Models;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;

namespace Assets.Scripts.Foenn.Engine.Filters
{
    public class GeoFilter : Filter
    {
        public GeoPoint point;

        public GeoFilter(GeoPoint point, AttributeKey filteredAttributeKey) : base(filteredAttributeKey)
        {
            this.point = point;
        }
    }
}