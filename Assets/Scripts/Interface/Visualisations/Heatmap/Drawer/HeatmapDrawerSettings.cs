using System;

namespace Assets.Scripts.Interface.Visualisations.Heatmap.Drawer
{
    public class HeatmapDrawerSettings
    {
        public readonly float alpha;// 0..1

        public readonly float tempMin;

        public readonly float tempMax;

        public HeatmapDrawerSettings(float alpha, float tempMin, float tempMax)
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
