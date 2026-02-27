using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class GeoFilter : Filter
    {
        public GeoPoint point;

        public GeoFilter(GeoPoint point, WeatherHistoryAttributeKey filteredAttributeKey) : base()
        {
            this.point = point;
        }
    }
}