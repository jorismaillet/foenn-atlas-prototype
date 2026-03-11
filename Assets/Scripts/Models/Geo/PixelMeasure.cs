namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public readonly struct PixelMeasure
    {
        public readonly PixelPoint point;

        public readonly float value;

        public PixelMeasure(PixelPoint point, float value)
        {
            this.point = point;
            this.value = value;
        }
    }
}
