namespace Assets.Scripts.Models.Geo
{
    public class GeoPoint
    {
        public readonly float lat, lon;

        public GeoPoint(float lat = 0, float lon = 0)
        {
            this.lat = lat;
            this.lon = lon;
        }
    }
}
