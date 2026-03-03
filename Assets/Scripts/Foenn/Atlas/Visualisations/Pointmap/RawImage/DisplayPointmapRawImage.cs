using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap.RawImage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    public class DisplayPointmapRawImage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] UnityEngine.UI.RawImage rawImage;

        [Header("RawImage Settings")]
        [SerializeField] Color pointColor = Color.red;
        [Range(0f, 1f)][SerializeField] float alpha = 1f;
        [Min(0)][SerializeField] int pointRadiusPx = 3;

        public void Display(IReadOnlyList<GeoMeasure> measures, RenderSettings render)
        {
            var rawImageSettings = new PointmapRawImageSettings(pointColor, alpha, pointRadiusPx);
            var texture = PointmapGenerator.BuildRawImageTexture(measures, rawImageSettings, render);
            rawImage.texture = texture;
        }
    }
}
