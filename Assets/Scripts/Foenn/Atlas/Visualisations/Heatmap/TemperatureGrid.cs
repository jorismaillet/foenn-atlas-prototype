using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public class TemperatureGrid
    {
        public readonly float[] values;
        public readonly float[] nearestD2;

        public TemperatureGrid(IReadOnlyList<PixelMeasure> pixelMeasures, int[][] gridBucketsArray, HeatmapSettings settings, RenderSettings render, int cellSize, int gridCols, int gridRows)
        {
            int w = render.width;
            int h = render.height;

            this.values = new float[w * h];
            this.nearestD2 = new float[w * h];

            // Defaults: NaN means "no value" => transparent.
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = float.NaN;
                nearestD2[i] = float.NaN;
            }

            float maxR2 = settings.maxRadiusPx * settings.maxRadiusPx;
            float pwr = Mathf.Max(0.1f, settings.idwPower);
            int maxN = Mathf.Max(1, settings.maxNeighbors);
            int k = Mathf.Min(maxN, 64); // hard cap for stack arrays
            int rangeCells = Mathf.CeilToInt(settings.maxRadiusPx / cellSize);

            // Reuse buffers to avoid per-pixel allocations (GC/perf killer).
            var bestD2 = new float[64];
            var bestI = new int[64];

            for (int y = 0; y < h; y++)
            {
                int cy = y / cellSize;

                for (int x = 0; x < w; x++)
                {
                    int idxPix = y * w + x;

                    int cx = x / cellSize;
                    int bestCount = 0;

                    if (TryAccumulateNeighborBuckets(gridBucketsArray, pixelMeasures, x, y, cx, cy, rangeCells, gridCols, gridRows, maxR2, k, bestD2, bestI, ref bestCount, out var exactValue))
                    {
                        values[idxPix] = exactValue;
                        nearestD2[idxPix] = 0f;
                        continue;
                    }

                    if (bestCount == 0)
                        continue;

                    values[idxPix] = ComputeIdwTemperature(pixelMeasures, bestD2, bestI, bestCount, pwr);
                    nearestD2[idxPix] = bestD2[0];
                }
            }
        }

        private bool TryAccumulateNeighborBuckets(
            int[][] gridBucketsArray,
            IReadOnlyList<PixelMeasure> pixelMeasures,
            int x,
            int y,
            int cx,
            int cy,
            int rangeCells,
            int gridCols,
            int gridRows,
            float maxR2,
            int k,
            float[] bestD2,
            int[] bestI,
            ref int bestCount,
            out float exactValue
        )
        {
            exactValue = 0f;

            // Parcourt les cellules voisines (verticalement) autour du pixel dans la grille de spatial-hash.
            for (int oy = -rangeCells; oy <= rangeCells; oy++)
            {
                int ny = cy + oy;
                if (ny < 0 || ny >= gridRows) continue;

                // Parcourt les cellules voisines (horizontalement) et récupère les points candidats.
                for (int ox = -rangeCells; ox <= rangeCells; ox++)
                {
                    int nx = cx + ox;
                    if (nx < 0 || nx >= gridCols) continue;

                    int key = ny * gridCols + nx;
                    var arr = gridBucketsArray[key];
                    if (arr == null) continue;

                    // Parcourt tous les points (stations) contenus dans cette cellule et met à jour le top-K voisins.
                    if (TryAccumulateBucketCandidates(arr, pixelMeasures, x, y, maxR2, k, bestD2, bestI, ref bestCount, out exactValue))
                        return true;
                }
            }

            return false;
        }

        private float ComputeIdwTemperature(
            IReadOnlyList<PixelMeasure> pixelMeasures,
            float[] bestD2,
            int[] bestI,
            int bestCount,
            float pwr
        )
        {
            double wSum = 0.0;
            double tSum = 0.0;

            const double eps = 1e-6;
            bool pwrIs2 = Mathf.Abs(pwr - 2f) < 1e-6f;

            // Combine les K voisins via IDW (pondération par distance) pour produire la valeur finale.
            for (int bi = 0; bi < bestCount; bi++)
            {
                double w;
                if (pwrIs2)
                {
                    // Optimisation: (sqrt(d2))^2 == d2 => évite Sqrt + Pow.
                    w = 1.0 / (bestD2[bi] + eps);
                }
                else
                {
                    float d = Mathf.Sqrt(bestD2[bi]);
                    w = 1.0 / (Math.Pow(d, pwr) + eps);
                }
                wSum += w;
                tSum += pixelMeasures[bestI[bi]].value * w;
            }

            return (float)(tSum / wSum);
        }

        private bool TryAccumulateBucketCandidates(
            int[] bucketPointIndices,
            IReadOnlyList<PixelMeasure> pixelMeasures,
            int x,
            int y,
            float maxR2,
            int k,
            float[] bestD2,
            int[] bestI,
            ref int bestCount,
            out float exactValue
        )
        {
            exactValue = 0f;

            for (int ai = 0; ai < bucketPointIndices.Length; ai++)
            {
                int pi = bucketPointIndices[ai];

                float dx = pixelMeasures[pi].point.x - x;
                float dy = pixelMeasures[pi].point.y - y;
                float d2 = dx * dx + dy * dy;

                if (d2 > maxR2) continue;

                if (d2 < 1e-6f)
                {
                    exactValue = pixelMeasures[pi].value;
                    return true;
                }

                if (bestCount < k)
                {
                    int pos = bestCount;
                    while (pos > 0 && bestD2[pos - 1] > d2)
                    {
                        bestD2[pos] = bestD2[pos - 1];
                        bestI[pos] = bestI[pos - 1];
                        pos--;
                    }
                    bestD2[pos] = d2;
                    bestI[pos] = pi;
                    bestCount++;
                }
                else if (d2 < bestD2[bestCount - 1])
                {
                    int pos = bestCount - 1;
                    while (pos > 0 && bestD2[pos - 1] > d2)
                    {
                        bestD2[pos] = bestD2[pos - 1];
                        bestI[pos] = bestI[pos - 1];
                        pos--;
                    }
                    bestD2[pos] = d2;
                    bestI[pos] = pi;
                }
            }

            return false;
        }
    }
}
