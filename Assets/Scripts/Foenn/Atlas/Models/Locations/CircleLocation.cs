namespace Assets.Scripts.Foenn.Atlas.Models.Locations
{
    public class CircleLocation : Location
    {
        public PointLocation center;
        public float radiusMeters;

        public CircleLocation(string name, PointLocation center, float radiusMeters) : base(name)
        {
            this.center = center;
            this.radiusMeters = radiusMeters;
        }
    }
}
