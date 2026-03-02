using System;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public static class RenderOperation
    {
        public static float LonToX(float lon, Visualisations.RenderSettings settings)
        {
            float denom = settings.bBox.maxLon - settings.bBox.minLon;
            if (denom <= 0f) return float.NaN;
            return (lon - settings.bBox.minLon) / denom * (settings.width - 1);
        }

        public static float LatToY(float lat, Visualisations.RenderSettings settings)
        {
            float denom = settings.bBox.maxLat - settings.bBox.minLat;
            if (denom <= 0f) return float.NaN;
            float t = (lat - settings.bBox.minLat) / denom;
            return 1f + t * (settings.height - 1);
        }

        public static Color32[] ReadMaskPixels(Texture2D mapMask, Visualisations.RenderSettings settings)
        {
            if (mapMask == null) return null;
            if (mapMask.width != settings.width || mapMask.height != settings.height)
                throw new ArgumentException($"Mask size must match {mapMask.width}x{mapMask.height} size.");
            return mapMask.GetPixels32();
        }

        public static Texture2D CreateTexture(Color32[] pixels, Visualisations.RenderSettings settings)
        {
            var tex = new Texture2D(settings.width, settings.height, TextureFormat.RGBA32, false, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.SetPixels32(pixels);
            tex.Apply(false, false);
            return tex;
        }
    }
}
