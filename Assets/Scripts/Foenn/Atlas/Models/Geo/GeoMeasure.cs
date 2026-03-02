namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public readonly struct GeoMeasure
    {
        public readonly GeoPoint point;
        public readonly float value;

        public GeoMeasure(GeoPoint point, float value)
        {
            this.point = point;
            this.value = value;
        }
    }
}
