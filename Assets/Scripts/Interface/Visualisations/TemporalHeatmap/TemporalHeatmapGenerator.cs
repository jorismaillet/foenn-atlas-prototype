using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations.TemporalHeatmap
{
    public class TemporalHeatmapGenerator
    {
        public static Texture2D BuildTemperatureTemporalHeatmap(List<float> hourlyObservations, CustomGradient gradient)
        {
            int width = 24;
            int height = hourlyObservations.Count / width;

            var pixels = new Color32[width * height];

            for (int i = 0; i < hourlyObservations.Count; i++)
            {
                pixels[i] = gradient.GetColor(hourlyObservations[i], 1f);
            }

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }
    }
}
