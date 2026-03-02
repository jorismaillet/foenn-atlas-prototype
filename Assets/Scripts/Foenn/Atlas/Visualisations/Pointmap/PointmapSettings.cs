using System;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    public readonly struct PointmapSettings
    {
        public static readonly Color defaultColor = new Color(1f, 0f, 0f);

        public readonly RenderSettings render;
        public readonly float alpha; // 0..1
        public readonly int pointRadiusPx;
        public readonly Color pointColor;

        public PointmapSettings(RenderSettings render, Color pointColor, float alpha = 1f, int pointRadiusPx = 3)
        {
            this.render = render;
            this.alpha = alpha;
            this.pointRadiusPx = pointRadiusPx;
            this.pointColor = pointColor;
        }

        public void Validate()
        {
            render.Validate();
            if (alpha < 0f || alpha > 1f)
                throw new ArgumentException("Invalid alpha.");

            if (pointRadiusPx < 0)
                throw new ArgumentException("Invalid pointRadiusPx.");
        }
    }
}
