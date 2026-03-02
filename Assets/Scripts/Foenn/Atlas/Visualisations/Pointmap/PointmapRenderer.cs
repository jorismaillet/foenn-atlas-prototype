using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
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
            var outPixels = new Color32[s.render.width * s.render.height];

            Color c = s.pointColor;
            c.a *= s.alpha;
            Color32 src = c;

            int r = Mathf.Max(0, s.pointRadiusPx);
            int r2 = r * r;

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                DrawFilledCircle(outPixels, s.render, maskPixels, p.x, p.y, r, r2, src);
            }

            return outPixels;
        }

        static void DrawFilledCircle(
            Color32[] outPixels,
            RenderSettings render,
            Color32[] maskPixels,
            int cx,
            int cy,
            int r,
            int r2,
            Color32 src
        )
        {
            int yMin = Math.Max(0, cy - r);
            int yMax = Math.Min(render.height - 1, cy + r);
            int xMin = Math.Max(0, cx - r);
            int xMax = Math.Min(render.width - 1, cx + r);

            for (int y = yMin; y <= yMax; y++)
            {
                int dy = y - cy;
                int dy2 = dy * dy;

                for (int x = xMin; x <= xMax; x++)
                {
                    int dx = x - cx;
                    if (dx * dx + dy2 > r2) continue;

                    int idx = y * render.width + x;

                    if (maskPixels != null && maskPixels[idx].r < 128)
                        continue;

                    outPixels[idx] = AlphaBlend(src, outPixels[idx]);
                }
            }
        }

        static Color32 AlphaBlend(Color32 src, Color32 dst)
        {
            // Standard "src over" alpha blending in byte space.
            int sa = src.a;
            if (sa <= 0) return dst;
            if (sa >= 255) return src;

            int da = dst.a;

            int invSa = 255 - sa;

            int outA = sa + (da * invSa + 127) / 255;

            int outR = (src.r * sa + dst.r * invSa + 127) / 255;
            int outG = (src.g * sa + dst.g * invSa + 127) / 255;
            int outB = (src.b * sa + dst.b * invSa + 127) / 255;

            return new Color32((byte)outR, (byte)outG, (byte)outB, (byte)outA);
        }
    }
}
