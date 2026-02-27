using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public class HeatmapGenerator
    {
        public static Texture2D BuildHeatmapTexture(
            IReadOnlyList<MeasurePoint> pointsLonLat,
            HeatmapSettings s,
            Texture2D franceMask = null // optional, same size, white=keep, black=hide
        )
        {
            if (s.width <= 0 || s.height <= 0) throw new ArgumentException("Invalid size.");
            if (pointsLonLat == null || pointsLonLat.Count == 0) throw new ArgumentException("No points.");

            // 1) Convert points to pixel-space for the grid
            var pts = new List<PixelPoint>(pointsLonLat.Count);
            for (int i = 0; i < pointsLonLat.Count; i++)
            {
                var p = pointsLonLat[i];
                if (float.IsNaN(p.value)) continue;

                float x = LonToX(p.lon, s);
                float y = LatToY(p.lat, s);

                // ignore points outside bbox
                if (x < 0 || x > s.width - 1 || y < 0 || y > s.height - 1) continue;

                pts.Add(new PixelPoint(x, y, p.value));
            }
            if (pts.Count < 3) throw new ArgumentException("Not enough points in bbox.");

            // 2) Build spatial hash: cell -> list of point indices
            int cellSize = Mathf.Max(4, s.cellSizePx);
            int gridCols = Mathf.CeilToInt(s.width / (float)cellSize);
            int gridRows = Mathf.CeilToInt(s.height / (float)cellSize);
            int cellCount = gridCols * gridRows;

            var buckets = new Dictionary<int, List<int>>(cellCount);

            for (int i = 0; i < pts.Count; i++)
            {
                int cx = Mathf.Clamp((int)(pts[i].x / cellSize), 0, gridCols - 1);
                int cy = Mathf.Clamp((int)(pts[i].y / cellSize), 0, gridRows - 1);
                int key = cy * gridCols + cx;

                if (!buckets.TryGetValue(key, out var list))
                {
                    list = new List<int>(8);
                    buckets[key] = list;
                }
                list.Add(i);
            }

            // 2bis) Flatten buckets into dense array (NO Dictionary in pixel loop)
            var bucketArrays = new int[cellCount][];
            foreach (var kv in buckets)
                bucketArrays[kv.Key] = kv.Value.ToArray();

            // 3) Prepare output pixels
            var outPixels = new Color32[s.width * s.height];

            // Optional mask pixels (read once)
            Color32[] maskPixels = null;
            if (franceMask != null)
            {
                if (franceMask.width != s.width || franceMask.height != s.height)
                    throw new ArgumentException("Mask size must match heatmap size.");
                maskPixels = franceMask.GetPixels32();
            }

            float maxR2 = s.maxRadiusPx * s.maxRadiusPx;
            float pwr = Mathf.Max(0.1f, s.idwPower);
            int maxN = Mathf.Max(1, s.maxNeighbors);
            int k = Mathf.Min(maxN, 64); // hard cap for stack arrays
            int rangeCells = Mathf.CeilToInt(s.maxRadiusPx / cellSize);
            byte aByte = (byte)Mathf.Clamp(Mathf.RoundToInt(s.alpha * 255f), 0, 255);

            // 4) For each pixel: scan nearby buckets, pick nearest neighbors, compute IDW
            for (int y = 0; y < s.height; y++)
            {
                int cy = y / cellSize;

                for (int x = 0; x < s.width; x++)
                {
                    int idxPix = y * s.width + x;

                    // Mask: black => transparent
                    if (maskPixels != null && maskPixels[idxPix].r < 128)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    int cx = x / cellSize;

                    // Keep K nearest within radius (simple insertion sorted by d2)
                    float[] bestD2 = new float[64];
                    int[] bestI = new int[64];
                    int bestCount = 0;

                    for (int oy = -rangeCells; oy <= rangeCells; oy++)
                    {
                        int ny = cy + oy;
                        if (ny < 0 || ny >= gridRows) continue;

                        for (int ox = -rangeCells; ox <= rangeCells; ox++)
                        {
                            int nx = cx + ox;
                            if (nx < 0 || nx >= gridCols) continue;

                            int key = ny * gridCols + nx;
                            var arr = bucketArrays[key];
                            if (arr == null) continue;

                            for (int ai = 0; ai < arr.Length; ai++)
                            {
                                int pi = arr[ai];

                                float dx = pts[pi].x - x;
                                float dy = pts[pi].y - y;
                                float d2 = dx * dx + dy * dy;

                                if (d2 > maxR2) continue;

                                // Perfect hit (pixel = station location)
                                if (d2 < 1e-6f)
                                {
                                    outPixels[idxPix] = TempToColor(pts[pi].value, s.tempMin, s.tempMax, aByte);
                                    goto nextPixel;
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
                        }
                    }

                    if (bestCount == 0)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        goto nextPixel;
                    }

                    // IDW
                    double wSum = 0.0;
                    double tSum = 0.0;

                    for (int bi = 0; bi < bestCount; bi++)
                    {
                        float d = Mathf.Sqrt(bestD2[bi]);
                        double w = 1.0 / (Math.Pow(d, pwr) + 1e-6);
                        wSum += w;
                        tSum += pts[bestI[bi]].value * w;
                    }

                    float temp = (float)(tSum / wSum);
                    outPixels[idxPix] = TempToColor(temp, s.tempMin, s.tempMax, aByte);

                nextPixel:;
                }
            }

            // 5) Create texture
            var tex = new Texture2D(s.width, s.height, TextureFormat.RGBA32, false, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.SetPixels32(outPixels);
            tex.Apply(false, false);
            return tex;
        }

        // ----- Coordinate mapping -----
        static float LonToX(float lon, in HeatmapSettings s)
            => (lon - s.bBox.minLon) / (s.bBox.maxLon - s.bBox.minLon) * (s.width - 1);

        static float LatToY(float lat, in HeatmapSettings s)
            => (s.bBox.maxLat - lat) / (s.bBox.maxLat - s.bBox.minLat) * (s.height - 1); // y inverted (north at top)

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