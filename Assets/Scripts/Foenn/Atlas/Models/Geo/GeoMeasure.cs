namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public class GeoMeasure
    {
        public GeoPoint point;
        public float value;

        public GeoMeasure(GeoPoint point, float value)
        {
            this.point = point;
            this.value = value;
        }
    }
}
