using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public class HeatmapGenerator
    {
        public static Texture2D BuildHeatmapTexture(
            IReadOnlyList<GeoMeasure> geoMeasures,
            HeatmapSettings s,
            Texture2D mapMask = null // optional, same size, white=keep, black=hide
        )
        {
            ValidateInputs(geoMeasures, s);

            var pixelMeasures = geoMeasures.ToPixelMeasures(s.render).ToList();
            if (pixelMeasures.Count < 3) throw new ArgumentException("Not enough points in bbox.");

            int cellSize = Mathf.Max(4, s.cellSizePx);
            int gridCols = Mathf.CeilToInt(s.render.width / (float)cellSize);
            int gridRows = Mathf.CeilToInt(s.render.height / (float)cellSize);

            var gridBucketsArray = BuildGridBucketsArray(pixelMeasures, cellSize, gridCols, gridRows);
            var maskPixels = RenderOperation.ReadMaskPixels(mapMask, s.render);
            var outPixels = HeatmapRenderer.RenderHeatmapPixels(pixelMeasures, gridBucketsArray, s, cellSize, gridCols, gridRows, maskPixels);

            return RenderOperation.CreateTexture(outPixels, s.render);
        }

        static void ValidateInputs(IReadOnlyList<GeoMeasure> geoMeasures, HeatmapSettings s)
        {
            s.Validate();
            if (geoMeasures == null || geoMeasures.Count == 0) throw new ArgumentException("No points.");
        }

        static int[][] BuildGridBucketsArray(IReadOnlyList<PixelMeasure> pixelMeasures, int cellSize, int gridCols, int gridRows)
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