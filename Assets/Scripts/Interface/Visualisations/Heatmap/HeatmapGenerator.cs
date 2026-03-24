using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interface.Visualisations.Heatmap.Drawer;
using Assets.Scripts.Interface.Visualisations.Heatmap.Render;
using Assets.Scripts.Interface.Visualisations.Tiles;
using Assets.Scripts.Models.Geo;
using UnityEngine;
using RenderSettings = Assets.Scripts.Interface.Visualisations.Heatmap.Render.RenderSettings;

namespace Assets.Scripts.Interface.Visualisations.Heatmap
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

            int fullSizePx = gridSize * TileGridHelper.TileSize;
            int sizePx = (targetTextureSizePx > 0) ? Mathf.Min(fullSizePx, targetTextureSizePx) : fullSizePx;
            sizePx = Mathf.Max(16, sizePx);
            var render = new RenderSettings(sizePx, sizePx, new BBox(0f, 0f, 1f, 1f));

            var fullPixelMeasures = geoMeasures.ToTileGridPixelMeasures(mapCenter, zoom, gridSize).ToList();
            if (fullPixelMeasures.Count < 3)
                return RenderOperation.CreateTexture(new Color32[render.width * render.height], render);

            // Downscale measures and pixel-based settings if we render at a lower resolution than the tile grid.
            float scale = (fullSizePx <= 1 || sizePx == fullSizePx) ? 1f : (sizePx - 1) / (float)(fullSizePx - 1);

            var pixelMeasures = (scale == 1f) ? fullPixelMeasures : DownscalePixelMeasures(fullPixelMeasures, scale);

            // Keep world-space look consistent: when rendering at lower resolution, pixel distances shrink.
            // So radius/cell settings must be scaled down with the same factor.
            var scaledSettings = (scale == 1f)
                ? settings
                : new HeatmapSettings(settings.idwPower, settings.maxNeighbors, Mathf.Max(1f, settings.maxRadiusPx * scale), Mathf.Max(1, Mathf.RoundToInt(settings.cellSizePx * scale)));

            int cellSize = Mathf.Max(2, scaledSettings.cellSizePx);
            int gridCols = Mathf.CeilToInt(render.width / (float)cellSize);
            int gridRows = Mathf.CeilToInt(render.height / (float)cellSize);

            FlattenPixelMeasures(pixelMeasures, out var xs, out var ys, out var vals);
            var spatialIndex = CsrSpatialIndex.Build(xs, ys, cellSize, gridCols, gridRows);
            var maskPixels = ReadMaskPixelsForTileGrid(mapMask, render, mapCenter, zoom, gridSize, maskBBox, reprojectMaskToTileGrid);

            // Direct render: compute IDW + color in one pass (no full-resolution TemperatureGrid buffers).
            var outPixels = HeatmapDrawer.RenderHeatmapPixels(xs, ys, vals, spatialIndex, scaledSettings, rawImageSettings, render, cellSize, maskPixels);
            var texture = RenderOperation.CreateTexture(outPixels, render);
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
        }

        static void FlattenPixelMeasures(IReadOnlyList<PixelMeasure> pixelMeasures, out int[] xs, out int[] ys, out float[] vals)
        {
            int n = pixelMeasures.Count;
            xs = new int[n];
            ys = new int[n];
            vals = new float[n];
            for (int i = 0; i < n; i++)
            {
                var pm = pixelMeasures[i];
                xs[i] = pm.point.x;
                ys[i] = pm.point.y;
                vals[i] = pm.value;
            }
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

            // Precompute srcY lookup (only depends on y)
            int[] srcYLut = new int[targetHeight];
            bool[] validY = new bool[targetHeight];
            for (int y = 0; y < targetHeight; y++)
            {
                int yFromTop = (targetHeight - 1) - y;
                double tileY = topTileY + ((yFromTop + 0.5) * gridSize / (double)targetHeight);
                double lat = TileGridHelper.TileYToLat(tileY, zoom);
                float v = (float)((lat - maskBBox.minLat) / latDenom);
                if (v >= 0f && v <= 1f)
                {
                    srcYLut[y] = Mathf.Clamp(Mathf.RoundToInt(v * (srcH - 1)), 0, srcH - 1);
                    validY[y] = true;
                }
            }

            // Precompute srcX lookup (only depends on x)
            int[] srcXLut = new int[targetWidth];
            bool[] validX = new bool[targetWidth];
            for (int x = 0; x < targetWidth; x++)
            {
                double tileX = leftTileX + ((x + 0.5) * gridSize / (double)targetWidth);
                double lon = TileGridHelper.TileXToLon(tileX, zoom);
                float u = (float)((lon - maskBBox.minLon) / lonDenom);
                if (u >= 0f && u <= 1f)
                {
                    srcXLut[x] = Mathf.Clamp(Mathf.RoundToInt(u * (srcW - 1)), 0, srcW - 1);
                    validX[x] = true;
                }
            }

            for (int y = 0; y < targetHeight; y++)
            {
                if (!validY[y])
                    continue;

                int srcRow = srcYLut[y] * srcW;
                int outRow = y * targetWidth;

                for (int x = 0; x < targetWidth; x++)
                {
                    if (!validX[x])
                        continue;

                    outMask[outRow + x] = src[srcRow + srcXLut[x]];
                }
            }

            return outMask;
        }
    }
}
