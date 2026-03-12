using Assets.Scripts.Models.Locations;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.Models.Geo
{
    public class GeoMeasure
    {
        public PointLocation point;

        public Field field;
        public float value;

        public GeoMeasure(PointLocation point, Field field, float value)
        {
            this.point = point;
            this.field = field;
            this.value = value;
        }
    }
}
