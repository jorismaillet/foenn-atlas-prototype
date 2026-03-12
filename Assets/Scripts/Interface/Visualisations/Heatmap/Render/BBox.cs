namespace Assets.Scripts.Components.Visualisations.Heatmap.Render
{
    public struct BBox
    {
        public static BBox France = new BBox(-5.5F, 41.0F, 20.0F, 51.5F);

        public float minLon, minLat, maxLon, maxLat;

        public BBox(float minLon, float minLat, float maxLon, float maxLat)
        {
            this.minLon = minLon;
            this.minLat = minLat;
            this.maxLon = maxLon;
            this.maxLat = maxLat;
        }
    }
}
