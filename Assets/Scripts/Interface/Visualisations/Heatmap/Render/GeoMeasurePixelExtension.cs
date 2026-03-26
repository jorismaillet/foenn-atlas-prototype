using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models.Geo;

namespace Assets.Scripts.Interface.Visualisations.Heatmap.Render
{
    public static class GeoMeasurePixelExtension
    {
        public static IEnumerable<PixelMeasure> ToTileGridPixelMeasures(this List<GeoMeasure> geoMeasures, GeoPoint mapCenter, int zoom, int gridSize)
        {
            int halfGridSize = gridSize / 2;

            double centerTileXf = TileGridHelper.LonToTileX(mapCenter.lon, zoom);
            double centerTileYf = TileGridHelper.LatToTileY(mapCenter.lat, zoom);

            int centerTileX = (int)Math.Floor(centerTileXf);
            int centerTileY = (int)Math.Floor(centerTileYf);

            double leftTileX = centerTileX - halfGridSize;
            double topTileY = centerTileY - halfGridSize;

            int width = gridSize * TileGridHelper.TileSize;
            int height = gridSize * TileGridHelper.TileSize;

            foreach (var geoMeasure in geoMeasures)
            {
                double tileX = TileGridHelper.LonToTileX(geoMeasure.point.lon, zoom);
                double tileY = TileGridHelper.LatToTileY(geoMeasure.point.lat, zoom);

                int x = (int)Math.Round((tileX - leftTileX) * TileGridHelper.TileSize);
                int yFromTop = (int)Math.Round((tileY - topTileY) * TileGridHelper.TileSize);
                int y = (height - 1) - yFromTop;

                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;

                yield return new PixelMeasure(new PixelPoint(x, y), geoMeasure.value);
            }
        }
    }
}
