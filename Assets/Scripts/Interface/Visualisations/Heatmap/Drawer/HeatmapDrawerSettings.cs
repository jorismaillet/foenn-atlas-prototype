using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations.Heatmap.Drawer
{
    public class HeatmapDrawerSettings
    {
        public readonly float alpha;// 0..1

        public readonly CustomGradient gradient;

        public HeatmapDrawerSettings(float alpha, CustomGradient fieldGradient)
        {
            this.gradient = fieldGradient;
        }

        public Color32 GetColor(float temp)
        {
            return gradient.GetColor(temp);
        }
    }
}
