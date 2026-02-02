using Assets.Scripts.Foenn.Engine;

namespace Assets.Scripts.Foenn.Atlas.Models.Locations {
    public class PointLocation : Location {
        public GeoPoint point;

        public PointLocation(string name, GeoPoint point) : base(name)
        {
            this.point = point;
        }
    }
}
