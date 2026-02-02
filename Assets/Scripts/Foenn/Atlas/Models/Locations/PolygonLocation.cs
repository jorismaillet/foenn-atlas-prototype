using Assets.Scripts.Foenn.Engine;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Locations {
    public class PolygonLocation : Location {
        public List<GeoPoint> points;

        public PolygonLocation(string name, params GeoPoint[] points) : base(name)
        {
            this.points = points.ToList();
            if(this.points.Count < 3) {
                throw new System.ArgumentException("A polygon location must have at least 3 points.");
            }
        }
    }
}
