using Assets.Scripts.Models.Locations;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.Models.Geo
{
    public class GeoMeasure
    {
        public readonly PointLocation point;

        public readonly Field field;

        public readonly float value;

        public GeoMeasure(PointLocation point, Field field, float value)
        {
            this.point = point;
            this.field = field;
            this.value = value;
        }
    }
}
