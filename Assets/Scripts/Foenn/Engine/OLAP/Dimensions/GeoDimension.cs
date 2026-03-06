using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions
{
    public class GeoDimension
    {
        public GeoPoint point;

        public GeoDimension(GeoPoint point)
        {
            this.point = point;
        }
    }
}