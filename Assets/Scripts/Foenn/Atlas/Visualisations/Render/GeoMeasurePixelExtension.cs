using Assets.Scripts.Foenn.Atlas.Layers;
using Assets.Scripts.Foenn.Atlas.Visualisations;
using System;
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
        }

        // Texture-space mapping aligned to the slippy tile grid.
        // Output is in pixels, for a texture sized (gridSize * 256) x (gridSize * 256), origin at top-left.
        public static IEnumerable<PixelPoint> ToTileGridPixelPoints(this IEnumerable<GeoMeasure> geoMeasures, GeoPoint mapCenter, int zoom, int gridSize)
        {
            int halfGridSize = gridSize / 2;

            double centerTileXf = SlippyMapMath.LonToTileX(mapCenter.lon, zoom);
            double centerTileYf = SlippyMapMath.LatToTileY(mapCenter.lat, zoom);

            int centerTileX = (int)Math.Floor(centerTileXf);
            int centerTileY = (int)Math.Floor(centerTileYf);

            double leftTileX = centerTileX - halfGridSize;
            double topTileY = centerTileY - halfGridSize;

            int width = gridSize * SlippyMapMath.TileSize;
            int height = gridSize * SlippyMapMath.TileSize;

            foreach (var geoMeasure in geoMeasures)
            {
                if (float.IsNaN(geoMeasure.value)) continue;

                double tileX = SlippyMapMath.LonToTileX(geoMeasure.point.lon, zoom);
                double tileY = SlippyMapMath.LatToTileY(geoMeasure.point.lat, zoom);

                int x = (int)Math.Round((tileX - leftTileX) * SlippyMapMath.TileSize);
                // Unity texture pixel arrays are addressed with (0,0) at the bottom-left,
                // while slippy tile Y grows downward from the top.
                int yFromTop = (int)Math.Round((tileY - topTileY) * SlippyMapMath.TileSize);
                int y = (height - 1) - yFromTop;

                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;

                yield return new PixelPoint(x, y);
            }
        }

        public static IEnumerable<PixelMeasure> ToTileGridPixelMeasures(this IEnumerable<GeoMeasure> geoMeasures, GeoPoint mapCenter, int zoom, int gridSize)
        {
            int halfGridSize = gridSize / 2;

            double centerTileXf = SlippyMapMath.LonToTileX(mapCenter.lon, zoom);
            double centerTileYf = SlippyMapMath.LatToTileY(mapCenter.lat, zoom);

            int centerTileX = (int)Math.Floor(centerTileXf);
            int centerTileY = (int)Math.Floor(centerTileYf);

            double leftTileX = centerTileX - halfGridSize;
            double topTileY = centerTileY - halfGridSize;

            int width = gridSize * SlippyMapMath.TileSize;
            int height = gridSize * SlippyMapMath.TileSize;

            foreach (var geoMeasure in geoMeasures)
            {
                if (float.IsNaN(geoMeasure.value)) continue;

                double tileX = SlippyMapMath.LonToTileX(geoMeasure.point.lon, zoom);
                double tileY = SlippyMapMath.LatToTileY(geoMeasure.point.lat, zoom);

                int x = (int)Math.Round((tileX - leftTileX) * SlippyMapMath.TileSize);
                int yFromTop = (int)Math.Round((tileY - topTileY) * SlippyMapMath.TileSize);
                int y = (height - 1) - yFromTop;

                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;

                yield return new PixelMeasure(new PixelPoint(x, y), geoMeasure.value);
            }
        }

        static bool TryToPixelPoint(GeoPoint geoPoint, RenderSettings render, out PixelPoint pixelPoint)
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
