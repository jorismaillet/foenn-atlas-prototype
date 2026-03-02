using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    internal static class HeatmapRenderer
    {
        internal static Color32[] RenderHeatmapPixels(
            IReadOnlyList<PixelMeasure> pixelMeasures,
            int[][] gridBucketsArray,
            HeatmapSettings s,
            int cellSize,
            int gridCols,
            int gridRows,
            Color32[] maskPixels
        )
        {
            var outPixels = new Color32[s.render.width * s.render.height];

            float maxR2 = s.maxRadiusPx * s.maxRadiusPx;
            float pwr = Mathf.Max(0.1f, s.idwPower);
            int maxN = Mathf.Max(1, s.maxNeighbors);
            int k = Mathf.Min(maxN, 64); // hard cap for stack arrays
            int rangeCells = Mathf.CeilToInt(s.maxRadiusPx / cellSize);
            byte aByte = (byte)Mathf.Clamp(Mathf.RoundToInt(s.alpha * 255f), 0, 255);

            // Reuse buffers to avoid per-pixel allocations (GC/perf killer).
            var bestD2 = new float[64];
            var bestI = new int[64];

            // Parcourt chaque ligne de l'image (axe Y) et calcule la couleur de tous les pixels de la ligne.
            for (int y = 0; y < s.render.height; y++)
            {
                int cy = y / cellSize;

                // Parcourt chaque pixel de la ligne (axe X), applique le masque puis interpole la valeur à cet endroit.
                for (int x = 0; x < s.render.width; x++)
                {
                    int idxPix = y * s.render.width + x;

                    if (maskPixels != null && maskPixels[idxPix].r < 128)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    int cx = x / cellSize;
                    int bestCount = 0;

                    // Parcourt les cellules voisines dans la grille de spatial-hash et met à jour le top-K voisins.
                    if (TryAccumulateNeighborBuckets(gridBucketsArray, pixelMeasures, x, y, cx, cy, rangeCells, gridCols, gridRows, maxR2, k, bestD2, bestI, ref bestCount, out var exactValue))
                    {
                        outPixels[idxPix] = TempToColor(exactValue, s.tempMin, s.tempMax, aByte);
                        continue;
                    }

                    if (bestCount == 0)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    float temp = ComputeIdwTemperature(pixelMeasures, bestD2, bestI, bestCount, pwr);
                    outPixels[idxPix] = TempToColor(temp, s.tempMin, s.tempMax, aByte);
                }
            }

            return outPixels;
        }

        static bool TryAccumulateNeighborBuckets(
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

        static float ComputeIdwTemperature(
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

        static bool TryAccumulateBucketCandidates(
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

        // ----- Color mapping -----
        static Color32 TempToColor(float temp, float tMin, float tMax, byte alpha)
        {
            float a = (temp - tMin) / (tMax - tMin + 1e-9f);
            a = Mathf.Clamp01(a);

            byte r, g, b;

            if (a < 0.33f)
            {
                float k = a / 0.33f;
                r = 0;
                g = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * k), 0, 255);
                b = 255;
            }
            else if (a < 0.66f)
            {
                float k = (a - 0.33f) / 0.33f;
                r = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * k), 0, 255);
                g = 255;
                b = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * (1f - k)), 0, 255);
            }
            else
            {
                float k = (a - 0.66f) / 0.34f;
                r = 255;
                g = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * (1f - k)), 0, 255);
                b = 0;
            }

            return new Color32(r, g, b, alpha);
        }
    }
}
