using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap.RawImage;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    internal static class HeatmapRawImageDrawer
    {
        internal static Color32[] ColorizeAverageGrid(TemperatureGrid grid, RenderSettings render, HeatmapSettings settings, HeatmapRawImageSettings rawImageSettings)
        {
            var outPixels = new Color32[render.width * render.height];

            byte baseAlpha = (byte)Mathf.Clamp(Mathf.RoundToInt(rawImageSettings.alpha * 255f), 0, 255);

            for (int i = 0; i < outPixels.Length; i++)
            {
                float v = grid.values[i];
                if (float.IsNaN(v))
                {
                    outPixels[i] = new Color32(0, 0, 0, 0);
                    continue;
                }

                // Fade-out alpha with distance to the nearest neighbor.
                float nearestD2 = grid.nearestD2[i];
                float fade = 1f;
                if (!float.IsNaN(nearestD2) && nearestD2 > 0f)
                {
                    float nearestD = Mathf.Sqrt(nearestD2);
                    fade = 1f - (nearestD / (settings.maxRadiusPx + 1e-6f));
                    fade = Mathf.Clamp01(fade);
                }

                byte alphaByte = (byte)Mathf.Clamp(Mathf.RoundToInt(baseAlpha * fade), 0, 255);
                outPixels[i] = TempToColor(v, rawImageSettings.tempMin, rawImageSettings.tempMax, alphaByte);
            }

            return outPixels;
        }


        // ----- Color mapping -----
        static Color32 TempToColor(float temp, float tMin, float tMax, byte alpha)
        {
            float a = (temp - tMin) / (tMax - tMin + 1e-9f);
            a = Mathf.Clamp01(a);

            byte r, g, b;

            if (a < 0.33f)
            {
                float k = a / 0.33f;
                r = 0;
                g = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * k), 0, 255);
                b = 255;
            }
            else if (a < 0.66f)
            {
                float k = (a - 0.33f) / 0.33f;
                r = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * k), 0, 255);
                g = 255;
                b = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * (1f - k)), 0, 255);
            }
            else
            {
                float k = (a - 0.66f) / 0.34f;
                r = 255;
                g = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * (1f - k)), 0, 255);
                b = 0;
            }

            return new Color32(r, g, b, alpha);
        }
    }
}
