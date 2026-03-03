using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap.RawImage;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    public static class PointmapGenerator
    {
        public static Texture2D BuildRawImageTexture(IReadOnlyList<GeoMeasure> geoMeasures, PointmapRawImageSettings rawImageSettings, RenderSettings renderSettings)
        {
            ValidateInput(geoMeasures);
            rawImageSettings.Validate();
            var pixelPoints = geoMeasures.ToPixelPoints(renderSettings).ToList();
            var coloredPixels = PointmapRawImageDrawer.RenderPointmapPixels(pixelPoints, rawImageSettings, renderSettings);
            var texture = RenderOperation.CreateTexture(coloredPixels, renderSettings);
            return texture;
        }

        static void ValidateInput(IReadOnlyList<GeoMeasure> geoMeasures)
        {
            if (geoMeasures == null || !geoMeasures.Any())
                throw new ArgumentException("No points.");
        }
    }
}
