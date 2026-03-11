namespace Assets.Scripts.Models.Locations
{
    public class CircleLocation : ILocation
    {
        public string name;

        public PointLocation center;

        public float radiusMeters;

        public CircleLocation(string name, PointLocation center, float radiusMeters)
        {
            this.name = name;
            this.center = center;
            this.radiusMeters = radiusMeters;
        }

        public string Name => this.name;
    }
}
