using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    internal static class PointmapRenderer
    {
        internal static Color32[] RenderPointmapPixels(
            IReadOnlyList<PixelPoint> points,
            PointmapSettings s,
            Color32[] maskPixels
        )
        {
            return PointmapTextureRenderer.RenderPointmapPixels(points, s, maskPixels);
        }
    }
}
