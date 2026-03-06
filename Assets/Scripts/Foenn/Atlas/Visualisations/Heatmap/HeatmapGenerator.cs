using Assets.Scripts.Foenn.Atlas.Layers;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap.RawImage;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public class HeatmapGenerator
    {
        public static Texture2D BuildTileGridRawImageTexture(
            List<GeoMeasure> geoMeasures,
            HeatmapSettings settings,
            GeoPoint mapCenter,
            int zoom,
            int gridSize,
            HeatmapDrawerSettings rawImageSettings,
            Texture2D mapMask = null,
            BBox maskBBox = default,
            bool reprojectMaskToTileGrid = true,
            int targetTextureSizePx = 0
        )
        {
            ValidateInputs(settings, rawImageSettings);

            var sw = new Stopwatch();
            sw.Start();
            int fullSizePx = gridSize * TileGridHelper.TileSize;
            int sizePx = (targetTextureSizePx > 0) ? Mathf.Min(fullSizePx, targetTextureSizePx) : fullSizePx;
            sizePx = Mathf.Max(16, sizePx);
            sw.Stop();
            MainThreadLog.Log($"1 {sw.ElapsedMilliseconds}");

            sw.Start();
            var render = new RenderSettings(sizePx, sizePx, new BBox(0f, 0f, 1f, 1f));

            sw.Stop();
            MainThreadLog.Log($"2 {sw.ElapsedMilliseconds}");
            sw.Start();
            var fullPixelMeasures = geoMeasures.ToTileGridPixelMeasures(mapCenter, zoom, gridSize).ToList();
            if (fullPixelMeasures.Count < 3)
                return RenderOperation.CreateTexture(new Color32[render.width * render.height], render);

            sw.Stop();
            MainThreadLog.Log($"3 {sw.ElapsedMilliseconds}");
            sw.Start();
            // Downscale measures and pixel-based settings if we render at a lower resolution than the tile grid.
            float scale = (fullSizePx <= 1 || sizePx == fullSizePx) ? 1f : (sizePx - 1) / (float)(fullSizePx - 1);

            var pixelMeasures = (scale == 1f) ? fullPixelMeasures : DownscalePixelMeasures(fullPixelMeasures, scale);

            sw.Stop();
            MainThreadLog.Log($"4 {sw.ElapsedMilliseconds}");
            sw.Start();
            // Keep world-space look consistent: pixel distances must scale with the render resolution.
            float invScale = (scale <= 0f) ? 1f : 1f / scale;
            var scaledSettings = (scale == 1f)
                ? settings
                : new HeatmapSettings(settings.idwPower, settings.maxNeighbors, settings.maxRadiusPx * invScale, Mathf.Max(1, Mathf.RoundToInt(settings.cellSizePx * invScale)));

            sw.Stop();
            MainThreadLog.Log($"5 {sw.ElapsedMilliseconds}");
            sw.Start();
            int cellSize = Mathf.Max(2, scaledSettings.cellSizePx);
            int gridCols = Mathf.CeilToInt(render.width / (float)cellSize);
            int gridRows = Mathf.CeilToInt(render.height / (float)cellSize);

            sw.Stop();
            MainThreadLog.Log($"6 {sw.ElapsedMilliseconds}");
            sw.Start();
            var spatialHash = BuildSpatialHash(pixelMeasures, cellSize, gridCols, gridRows);
            var maskPixels = ReadMaskPixelsForTileGrid(mapMask, render, mapCenter, zoom, gridSize, maskBBox, reprojectMaskToTileGrid);

            sw.Stop();
            MainThreadLog.Log($"7 {sw.ElapsedMilliseconds}");
            sw.Start();
            // Direct render: compute IDW + color in one pass (no full-resolution TemperatureGrid buffers).
            var outPixels = HeatmapDrawer.RenderHeatmapPixels(pixelMeasures, spatialHash, scaledSettings, rawImageSettings, render, cellSize, gridCols, gridRows, maskPixels);
            sw.Stop();
            MainThreadLog.Log($"8 {sw.ElapsedMilliseconds}");
            sw.Start();
            var texture = RenderOperation.CreateTexture(outPixels, render);
            sw.Stop();
            MainThreadLog.Log($"9 {sw.ElapsedMilliseconds}");
            return texture;
        }

        
        static List<PixelMeasure> DownscalePixelMeasures(List<PixelMeasure> fullPixelMeasures, float scale)
        {
            var scaled = new List<PixelMeasure>(fullPixelMeasures.Count);
            for (int i = 0; i < fullPixelMeasures.Count; i++)
            {
                var pm = fullPixelMeasures[i];
                int x = Mathf.RoundToInt(pm.point.x * scale);
                int y = Mathf.RoundToInt(pm.point.y * scale);
                scaled.Add(new PixelMeasure(new PixelPoint(x, y), pm.value));
            }
            return scaled;
        }

        private static void ValidateInputs(HeatmapSettings settings, HeatmapDrawerSettings rawImageSettings)
        {
            settings.Validate();
            rawImageSettings.Validate();
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
            double centerTileXf = TileGridHelper.LonToTileX(mapCenter.lon, zoom);
            double centerTileYf = TileGridHelper.LatToTileY(mapCenter.lat, zoom);
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
                double tileY = topTileY + ((yFromTop + 0.5) * gridSize / (double)targetHeight);

                double lat = TileGridHelper.TileYToLat(tileY, zoom);
                float v = (float)((lat - maskBBox.minLat) / latDenom);
                if (v < 0f || v > 1f)
                    continue;

                int srcY = Mathf.Clamp(Mathf.RoundToInt(v * (srcH - 1)), 0, srcH - 1);

                for (int x = 0; x < targetWidth; x++)
                {
                    double tileX = leftTileX + ((x + 0.5) * gridSize / (double)targetWidth);
                    double lon = TileGridHelper.TileXToLon(tileX, zoom);

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