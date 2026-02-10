using Assets.Scripts.Foenn.Atlas.Models.Geo;

namespace Assets.Scripts.Foenn.Atlas.Models.Locations
{
    public class RectangleLocation : Location
    {
        public GeoPoint center;
        public float widthMeters, heightMeters;

        public RectangleLocation(string name, GeoPoint center, float widthMeters, float heightMeters) : base(name)
        {
            this.center = center;
            this.widthMeters = widthMeters;
            this.heightMeters = heightMeters;
        }
    }
}
