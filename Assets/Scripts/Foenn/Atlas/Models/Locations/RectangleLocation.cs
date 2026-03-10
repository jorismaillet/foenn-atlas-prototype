namespace Assets.Scripts.Foenn.Atlas.Models.Locations
{
    using Assets.Scripts.Foenn.Atlas.Models.Geo;

    public class RectangleLocation : ILocation
    {
        public string name;

        public GeoPoint center;

        public float widthMeters, heightMeters;

        public RectangleLocation(string name, GeoPoint center, float widthMeters, float heightMeters)
        {
            this.name = name;
            this.center = center;
            this.widthMeters = widthMeters;
            this.heightMeters = heightMeters;
        }

        public string Name => this.name;
    }
}
