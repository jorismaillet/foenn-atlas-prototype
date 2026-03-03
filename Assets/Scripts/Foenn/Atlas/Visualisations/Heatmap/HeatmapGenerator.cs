using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Layers;
using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap.RawImage;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public class HeatmapGenerator
    {
        public static Texture2D BuildRawImageTexture(IReadOnlyList<GeoMeasure> geoMeasures, HeatmapSettings settings, RenderSettings render, HeatmapRawImageSettings rawImageSettings)
        {
            ValidateInputs(geoMeasures, settings, rawImageSettings);

            var pixelMeasures = geoMeasures.ToPixelMeasures(render).ToList();
            if (pixelMeasures.Count < 3) throw new ArgumentException("Not enough points in bbox.");

            int cellSize = Mathf.Max(4, settings.cellSizePx);
            int gridCols = Mathf.CeilToInt(render.width / (float)cellSize);
            int gridRows = Mathf.CeilToInt(render.height / (float)cellSize);

            var spatialHash = BuildSpatialHash(pixelMeasures, cellSize, gridCols, gridRows);
            var averageGrid = new TemperatureGrid(pixelMeasures, spatialHash, settings, render, cellSize, gridCols, gridRows);
            var outPixels = HeatmapRawImageDrawer.ColorizeAverageGrid(averageGrid, render, settings, rawImageSettings);
            var texture = RenderOperation.CreateTexture(outPixels, render);
            return texture;
        }

        public static Texture2D BuildTileGridRawImageTexture(
            IReadOnlyList<GeoMeasure> geoMeasures,
            HeatmapSettings settings,
            GeoPoint mapCenter,
            int zoom,
            int gridSize,
            HeatmapRawImageSettings rawImageSettings,
            Texture2D mapMask = null,
            BBox maskBBox = default,
            bool reprojectMaskToTileGrid = true
        )
        {
            ValidateInputs(geoMeasures, settings, rawImageSettings);

            int sizePx = gridSize * SlippyMapMath.TileSize;
            var render = new RenderSettings(sizePx, sizePx, new BBox(0f, 0f, 1f, 1f));

            var pixelMeasures = geoMeasures.ToTileGridPixelMeasures(mapCenter, zoom, gridSize).ToList();
            if (pixelMeasures.Count < 3)
                return RenderOperation.CreateTexture(new Color32[render.width * render.height], render);

            int cellSize = Mathf.Max(4, settings.cellSizePx);
            int gridCols = Mathf.CeilToInt(render.width / (float)cellSize);
            int gridRows = Mathf.CeilToInt(render.height / (float)cellSize);

            var spatialHash = BuildSpatialHash(pixelMeasures, cellSize, gridCols, gridRows);
            var averageGrid = new TemperatureGrid(pixelMeasures, spatialHash, settings, render, cellSize, gridCols, gridRows);
            var maskPixels = ReadMaskPixelsForTileGrid(mapMask, render, mapCenter, zoom, gridSize, maskBBox, reprojectMaskToTileGrid);
            var outPixels = HeatmapRawImageDrawer.ColorizeAverageGrid(averageGrid, render, settings, rawImageSettings, maskPixels);

            return RenderOperation.CreateTexture(outPixels, render);
        }

        private static void ValidateInputs(IReadOnlyList<GeoMeasure> geoMeasures, HeatmapSettings settings, HeatmapRawImageSettings rawImageSettings)
        {
            settings.Validate();
            rawImageSettings.Validate();
            if (geoMeasures == null || geoMeasures.Count == 0) throw new ArgumentException("No points.");
        }

        private static int[][] BuildSpatialHash(IReadOnlyList<PixelMeasure> pixelMeasures, int cellSize, int gridCols, int gridRows)
        {
            int cellCount = gridCols * gridRows;
            var gridBucketsList = new List<int>[cellCount];

            for (int pixelMeasureIndice = 0; pixelMeasureIndice < pixelMeasures.Count; pixelMeasureIndice++)
            {
                int cx = Mathf.Clamp(pixelMeasures[pixelMeasureIndice].point.x / cellSize, 0, gridCols - 1);
                int cy = Mathf.Clamp(pixelMeasures[pixelMeasureIndice].point.y / cellSize, 0, gridRows - 1);
                int bucketIdentifier = cy * gridCols + cx;

                var gridBucket = gridBucketsList[bucketIdentifier];
                if (gridBucket == null)
                {
                    gridBucket = new List<int>(8);
                    gridBucketsList[bucketIdentifier] = gridBucket;
                }
                gridBucket.Add(pixelMeasureIndice);
            }
            var gridBucketsArray = new int[cellCount][];
            for (int i = 0; i < gridBucketsList.Length; i++)
            {
                var bucket = gridBucketsList[i];
                if (bucket != null)
                    gridBucketsArray[i] = bucket.ToArray();
            }
            return gridBucketsArray;
        }

        static Color32[] ReadMaskPixelsForTileGrid(
            Texture2D mapMask,
            RenderSettings render,
            GeoPoint mapCenter,
            int zoom,
            int gridSize,
            BBox maskBBox,
            bool reprojectMaskToTileGrid
        )
        {
            if (mapMask == null) return null;

            // Default bbox if not provided.
            if (maskBBox.maxLon == 0f && maskBBox.maxLat == 0f && maskBBox.minLon == 0f && maskBBox.minLat == 0f)
                maskBBox = BBox.France;

            if (mapMask.width == render.width && mapMask.height == render.height)
                return RenderOperation.ReadMaskPixels(mapMask, render);

            if (!reprojectMaskToTileGrid)
                return null;

            var src = mapMask.GetPixels32();

            float lonDenom = maskBBox.maxLon - maskBBox.minLon;
            float latDenom = maskBBox.maxLat - maskBBox.minLat;
            if (lonDenom <= 0f || latDenom <= 0f)
                return null;

            int halfGridSize = gridSize / 2;
            double centerTileXf = SlippyMapMath.LonToTileX(mapCenter.lon, zoom);
            double centerTileYf = SlippyMapMath.LatToTileY(mapCenter.lat, zoom);
            int centerTileX = (int)Math.Floor(centerTileXf);
            int centerTileY = (int)Math.Floor(centerTileYf);
            double leftTileX = centerTileX - halfGridSize;
            double topTileY = centerTileY - halfGridSize;

            int targetWidth = render.width;
            int targetHeight = render.height;
            var outMask = new Color32[targetWidth * targetHeight];

            int srcW = mapMask.width;
            int srcH = mapMask.height;

            for (int y = 0; y < targetHeight; y++)
            {
                int yFromTop = (targetHeight - 1) - y;
                double tileY = topTileY + ((yFromTop + 0.5) / SlippyMapMath.TileSize);

                double lat = SlippyMapMath.TileYToLat(tileY, zoom);
                float v = (float)((lat - maskBBox.minLat) / latDenom);
                if (v < 0f || v > 1f)
                    continue;

                int srcY = Mathf.Clamp(Mathf.RoundToInt(v * (srcH - 1)), 0, srcH - 1);

                for (int x = 0; x < targetWidth; x++)
                {
                    double tileX = leftTileX + ((x + 0.5) / SlippyMapMath.TileSize);
                    double lon = SlippyMapMath.TileXToLon(tileX, zoom);

                    float u = (float)((lon - maskBBox.minLon) / lonDenom);
                    if (u < 0f || u > 1f)
                        continue;

                    int srcX = Mathf.Clamp(Mathf.RoundToInt(u * (srcW - 1)), 0, srcW - 1);
                    outMask[y * targetWidth + x] = src[srcY * srcW + srcX];
                }
            }

            return outMask;
        }
    }
}