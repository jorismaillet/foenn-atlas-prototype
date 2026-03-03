using Assets.Scripts.Foenn.Atlas.Models.Geo;
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
    }
}