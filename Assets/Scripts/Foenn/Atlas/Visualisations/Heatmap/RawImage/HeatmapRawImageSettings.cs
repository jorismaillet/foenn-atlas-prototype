using System;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap.RawImage
{
    public class HeatmapRawImageSettings
    {
        public readonly float alpha;          // 0..1 overlay alpha
        public readonly float tempMin;        // for color mapping
        public readonly float tempMax;        // for color mapping

        public HeatmapRawImageSettings(float alpha, float tempMin, float tempMax)
        {
            this.alpha = alpha;
            this.tempMin = tempMin;
            this.tempMax = tempMax;
        }

        public void Validate()
        {
            if (alpha < 0f || alpha > 1f)
                throw new ArgumentException("Invalid alpha.");

            if (tempMax <= tempMin)
                throw new ArgumentException("Invalid temperature range.");
        }
    }
}
