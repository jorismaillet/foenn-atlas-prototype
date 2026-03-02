namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public readonly struct GeoPoint
    {
        public readonly float lat, lon;

        public GeoPoint(float lat, float lon)
        {
            this.lat = lat;
            this.lon = lon;
        }
    }
}
