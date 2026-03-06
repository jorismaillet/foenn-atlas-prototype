using Assets.Scripts.Foenn.Engine.OLAP.Metrics;

namespace Assets.Scripts.Foenn.Atlas.Models.Geo
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
