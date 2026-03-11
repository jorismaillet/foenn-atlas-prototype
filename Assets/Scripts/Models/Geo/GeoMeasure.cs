using Assets.Scripts.OLAP.Engine.Result;

namespace Assets.Scripts.Models.Geo
{
    public class GeoMeasure
    {
        public GeoPoint point;

        public Measure measure;

        public GeoMeasure(GeoPoint point, Measure measure)
        {
            this.point = point;
            this.measure = measure;
        }
    }
}
