using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap.RawImage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public class DisplayHeatmapRawImage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] UnityEngine.UI.RawImage rawImage;

        [Header("RawImage Settings")]
        [Range(0f, 1f)][SerializeField] float alpha = 0.85f;
        [SerializeField] float tempMin = -10f;
        [SerializeField] float tempMax = 40f;

        public void Display(IReadOnlyList<GeoMeasure> measures, HeatmapSettings settings, RenderSettings render)
        {
            var rawImageSettings = new HeatmapRawImageSettings(alpha, tempMin, tempMax);
            var texture = HeatmapGenerator.BuildRawImageTexture(measures, settings, render, rawImageSettings);
            rawImage.texture = texture;
        }
    }
}
