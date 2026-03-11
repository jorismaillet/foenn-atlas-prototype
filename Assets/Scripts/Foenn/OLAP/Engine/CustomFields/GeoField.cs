namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using Assets.Scripts.Foenn.Atlas.Models.Geo;

    public class GeoField
    {
        public GeoPoint point;

        public GeoField(GeoPoint point) => this.point = point;

        public GeoField(float lat, float lon) => point = new GeoPoint(lat, lon);
    }
}
