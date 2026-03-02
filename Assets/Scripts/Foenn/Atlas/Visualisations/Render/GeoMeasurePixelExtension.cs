using Assets.Scripts.Foenn.Atlas.Visualisations;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public static class GeoMeasurePixelExtension
    {
        public static IEnumerable<PixelMeasure> ToPixelMeasures(this IEnumerable<GeoMeasure> geoMeasures, RenderSettings render)
        {
            foreach (var geoMeasure in geoMeasures)
            {
                if (float.IsNaN(geoMeasure.value)) continue;
                if (!TryToPixelPoint(geoMeasure.point, render, out var pixelPoint))
                    continue;
                yield return new PixelMeasure(pixelPoint, geoMeasure.value);
            }
            yield break;
        }

        public static IEnumerable<PixelPoint> ToPixelPoints(this IEnumerable<GeoMeasure> geoMeasures, RenderSettings render)
        {
            foreach (var geoMeasure in geoMeasures)
            {
                if (float.IsNaN(geoMeasure.value)) continue;
                if (!TryToPixelPoint(geoMeasure.point, render, out var pixelPoint))
                    continue;

                yield return pixelPoint;
            }
            yield break;
        }

        private static bool TryToPixelPoint(GeoPoint geoPoint, RenderSettings render, out PixelPoint pixelPoint)
        {
            float xf = RenderOperation.LonToX(geoPoint.lon, render);
            float yf = RenderOperation.LatToY(geoPoint.lat, render);

            int x = UnityEngine.Mathf.RoundToInt(xf);
            int y = UnityEngine.Mathf.RoundToInt(yf);

            if (x < 0 || x >= render.width || y < 0 || y >= render.height)
            {
                pixelPoint = default;
                return false;
            }

            pixelPoint = new PixelPoint(x, y);
            return true;
        }
    }
}
