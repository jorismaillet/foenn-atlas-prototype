using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;

namespace Assets.Scripts.Foenn.Atlas.Visualisations
{
    public readonly struct RenderSettings
    {
        public readonly int width;
        public readonly int height;
        public readonly BBox bBox;

        public RenderSettings(int width, int height, BBox bBox)
        {
            this.width = width;
            this.height = height;
            this.bBox = bBox;
        }

        public void Validate()
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Invalid size.");

            if (bBox.maxLon <= bBox.minLon || bBox.maxLat <= bBox.minLat)
                throw new ArgumentException("Invalid BBox.");
        }
    }
}
