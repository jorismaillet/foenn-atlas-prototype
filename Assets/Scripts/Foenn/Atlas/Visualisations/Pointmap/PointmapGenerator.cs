using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    public static class PointmapGenerator
    {
        public static Texture2D BuildPointmapTexture(
            IReadOnlyList<GeoMeasure> geoMeasures,
            PointmapSettings s,
            Texture2D mapMask = null
        )
        {
            ValidateInputs(geoMeasures, s);

            var pixelPoints = geoMeasures.ToPixelPoints(s.render).ToList();;
            var maskPixels = RenderOperation.ReadMaskPixels(mapMask, s.render);
            var outPixels = PointmapRenderer.RenderPointmapPixels(pixelPoints, s, maskPixels);
            return RenderOperation.CreateTexture(outPixels, s.render);
        }

        static void ValidateInputs(IReadOnlyList<GeoMeasure> geoMeasures, PointmapSettings s)
        {
            s.Validate();

            if (geoMeasures == null || geoMeasures.Count == 0)
                throw new ArgumentException("No points.");
        }
    }
}
