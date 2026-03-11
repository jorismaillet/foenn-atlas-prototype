using Assets.Scripts.Models.Geo;

namespace Assets.Scripts.Models.Locations
{
    public class PointLocation : GeoPoint, ILocation
    {
        public string name;

        public PointLocation(string name, float lat, float lon) : base(lat, lon)
        {
            this.name = name;
        }

        public string Name => this.name;
    }
}
