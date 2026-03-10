using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public class GeoMeasure
    {
        public GeoPoint point;
        public List<Measure> measures;

        public GeoMeasure(GeoPoint point, List<Measure> measures)
        {
            this.point = point;
            this.measures = measures;
        }
    }
}
