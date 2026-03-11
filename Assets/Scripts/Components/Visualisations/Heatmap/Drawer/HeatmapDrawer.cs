namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap.RawImage;
    using System;
    using UnityEngine;
    using RenderSettings = Assets.Scripts.Foenn.Atlas.Visualisations.RenderSettings;

    internal static class HeatmapDrawer
    {
        private const float ExactHitEpsilon = 1e-6f;

        private const double WeightEpsilon = 1e-6;

        public static Color32[] RenderHeatmapPixels(
            int[] xs,
            int[] ys,
            float[] vals,
            CsrSpatialIndex spatialIndex,
            HeatmapSettings settings,
            HeatmapDrawerSettings drawer,
            RenderSettings render,
            int cellSize,
            Color32[] maskPixels
        )
        {
            int w = render.width;
            int h = render.height;
            var outPixels = new Color32[w * h];

            if (xs == null || ys == null || vals == null || xs.Length == 0 || w <= 0 || h <= 0)
                return outPixels;

            float maxRadiusPx = settings.maxRadiusPx;
            float maxR2 = maxRadiusPx * maxRadiusPx;
            float pwr = Mathf.Max(0.1f, settings.idwPower);
            int maxN = Mathf.Max(1, settings.maxNeighbors);
            int k = Mathf.Min(maxN, 64);
            int rangeCells = Mathf.CeilToInt(maxRadiusPx / Mathf.Max(1, cellSize));
            byte baseAlpha = (byte)Mathf.Clamp(Mathf.RoundToInt(drawer.alpha * 255f), 0, 255);

            // ---- BIG perf lever ----
            // On calcule l'IDW sur une grille coarse, puis on upscale.
            // 1024² -> step 4 => ~256² samples => ~16x moins de boulot.
            int sampleStep = ChooseSampleStep(w, h, cellSize, xs.Length, settings.maxRadiusPx);

            if (sampleStep <= 1)
            {
                RenderFullResolution(
                    xs,
                    ys,
                    vals,
                    spatialIndex,
                    settings,
                    drawer,
                    render,
                    cellSize,
                    maskPixels,
                    outPixels,
                    maxR2,
                    pwr,
                    k,
                    rangeCells,
                    baseAlpha
                );
                return outPixels;
            }

            RenderUpscaledFromCoarseGrid(
                xs,
                ys,
                vals,
                spatialIndex,
                settings,
                drawer,
                render,
                cellSize,
                maskPixels,
                outPixels,
                maxR2,
                pwr,
                k,
                rangeCells,
                baseAlpha,
                sampleStep
            );

            return outPixels;
        }

        private static int ChooseSampleStep(int w, int h, int cellSize, int measureCount, float maxRadiusPx)
        {
            int maxDim = Mathf.Max(w, h);

            // Heuristics tuned for CPU cost: prefer fewer coarse samples when the field is smooth
            // (small measure count and/or large max radius).
            bool verySmooth = measureCount <= 64 || maxRadiusPx >= 140f;
            bool smooth = measureCount <= 128 || maxRadiusPx >= 100f;

            if (maxDim >= 2048)
                return verySmooth ? 12 : 8;

            if (maxDim >= 1024)
                return verySmooth ? 8 : (smooth ? 6 : 4);

            if (maxDim >= 512)
                return verySmooth ? 6 : (smooth ? 4 : 3);

            return smooth ? 3 : 2;
        }

        private static void RenderFullResolution(
            int[] xs,
            int[] ys,
            float[] vals,
            CsrSpatialIndex spatialIndex,
            HeatmapSettings settings,
            HeatmapDrawerSettings drawer,
            RenderSettings render,
            int cellSize,
            Color32[] maskPixels,
            Color32[] outPixels,
            float maxR2,
            float pwr,
            int k,
            int rangeCells,
            byte baseAlpha
        )
        {
            int w = render.width;
            int h = render.height;

            var bestD2 = new float[64];
            var bestI = new int[64];

            for (int y = 0; y < h; y++)
            {
                int cy = y / cellSize;
                int row = y * w;

                for (int x = 0; x < w; x++)
                {
                    int idxPix = row + x;

                    if (maskPixels != null && maskPixels[idxPix].r < 128)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    int cx = x / cellSize;
                    int bestCount = 0;

                    if (TryAccumulateNeighborBuckets(
                        spatialIndex,
                        xs,
                        ys,
                        vals,
                        x,
                        y,
                        cx,
                        cy,
                        rangeCells,
                        maxR2,
                        k,
                        bestD2,
                        bestI,
                        ref bestCount,
                        out float exactValue))
                    {
                        outPixels[idxPix] = TempToColor(exactValue, drawer.tempMin, drawer.tempMax, baseAlpha);
                        continue;
                    }

                    if (bestCount == 0)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    float temp = ComputeIdwTemperature(vals, bestD2, bestI, bestCount, pwr);

                    float nearestD = Mathf.Sqrt(bestD2[0]);
                    float fade = 1f - (nearestD / (settings.maxRadiusPx + 1e-6f));
                    fade = Mathf.Clamp01(fade);
                    byte alphaByte = (byte)Mathf.Clamp(Mathf.RoundToInt(baseAlpha * fade), 0, 255);

                    outPixels[idxPix] = TempToColor(temp, drawer.tempMin, drawer.tempMax, alphaByte);
                }
            }
        }

        private static void RenderUpscaledFromCoarseGrid(
            int[] xs,
            int[] ys,
            float[] vals,
            CsrSpatialIndex spatialIndex,
            HeatmapSettings settings,
            HeatmapDrawerSettings drawer,
            RenderSettings render,
            int cellSize,
            Color32[] maskPixels,
            Color32[] outPixels,
            float maxR2,
            float pwr,
            int k,
            int rangeCells,
            byte baseAlpha,
            int sampleStep
        )
        {
            int w = render.width;
            int h = render.height;

            int coarseW = Mathf.Max(2, Mathf.CeilToInt((float)w / sampleStep) + 1);
            int coarseH = Mathf.Max(2, Mathf.CeilToInt((float)h / sampleStep) + 1);

            var coarseTemp = new float[coarseW * coarseH];
            var coarseAlpha = new byte[coarseW * coarseH];

#if UNITY_2019_1_OR_NEWER
            // 1) Compute coarse field (Burst/Jobs when available)
            if (!HeatmapDrawerJobs.TryComputeCoarseField(
                xs,
                ys,
                vals,
                spatialIndex,
                settings,
                cellSize,
                maxR2,
                pwr,
                k,
                rangeCells,
                baseAlpha,
                w,
                h,
                coarseW,
                coarseH,
                sampleStep,
                coarseTemp,
                coarseAlpha))
#endif
            {
                // 1) Compute coarse field (managed fallback)
                var bestD2 = new float[64];
                var bestI = new int[64];

                for (int gy = 0; gy < coarseH; gy++)
                {
                    int y = Mathf.Min(h - 1, gy * sampleStep);
                    int cy = y / cellSize;
                    int coarseRow = gy * coarseW;

                    for (int gx = 0; gx < coarseW; gx++)
                    {
                        int x = Mathf.Min(w - 1, gx * sampleStep);
                        int cx = x / cellSize;
                        int bestCount = 0;

                        float temp = float.NaN;
                        byte alphaByte = 0;

                        if (TryAccumulateNeighborBuckets(
                            spatialIndex,
                            xs,
                            ys,
                            vals,
                            x,
                            y,
                            cx,
                            cy,
                            rangeCells,
                            maxR2,
                            k,
                            bestD2,
                            bestI,
                            ref bestCount,
                            out float exactValue))
                        {
                            temp = exactValue;
                            alphaByte = baseAlpha;
                        }
                        else if (bestCount > 0)
                        {
                            temp = ComputeIdwTemperature(vals, bestD2, bestI, bestCount, pwr);
                            float nearestD = Mathf.Sqrt(bestD2[0]);
                            float fade = 1f - (nearestD / (settings.maxRadiusPx + 1e-6f));
                            fade = Mathf.Clamp01(fade);
                            alphaByte = (byte)Mathf.Clamp(Mathf.RoundToInt(baseAlpha * fade), 0, 255);
                        }

                        coarseTemp[coarseRow + gx] = temp;
                        coarseAlpha[coarseRow + gx] = alphaByte;
                    }
                }
            }

            // 2) Bilinear upscale + final colorization
            for (int y = 0; y < h; y++)
            {
                int row = y * w;

                float gyf = y / (float)sampleStep;
                int gy0 = Mathf.Clamp((int)gyf, 0, coarseH - 1);
                int gy1 = Mathf.Clamp(gy0 + 1, 0, coarseH - 1);
                float ty = Mathf.Clamp01(gyf - gy0);

                int coarseRow0 = gy0 * coarseW;
                int coarseRow1 = gy1 * coarseW;

                for (int x = 0; x < w; x++)
                {
                    int idxPix = row + x;

                    if (maskPixels != null && maskPixels[idxPix].r < 128)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    float gxf = x / (float)sampleStep;
                    int gx0 = Mathf.Clamp((int)gxf, 0, coarseW - 1);
                    int gx1 = Mathf.Clamp(gx0 + 1, 0, coarseW - 1);
                    float tx = Mathf.Clamp01(gxf - gx0);

                    int i00 = coarseRow0 + gx0;
                    int i10 = coarseRow0 + gx1;
                    int i01 = coarseRow1 + gx0;
                    int i11 = coarseRow1 + gx1;

                    bool v00 = !float.IsNaN(coarseTemp[i00]);
                    bool v10 = !float.IsNaN(coarseTemp[i10]);
                    bool v01 = !float.IsNaN(coarseTemp[i01]);
                    bool v11 = !float.IsNaN(coarseTemp[i11]);

                    int validCount = 0;
                    float sumTemp = 0f;
                    int sumAlpha = 0;

                    if (v00) { sumTemp += coarseTemp[i00]; sumAlpha += coarseAlpha[i00]; validCount++; }
                    if (v10) { sumTemp += coarseTemp[i10]; sumAlpha += coarseAlpha[i10]; validCount++; }
                    if (v01) { sumTemp += coarseTemp[i01]; sumAlpha += coarseAlpha[i01]; validCount++; }
                    if (v11) { sumTemp += coarseTemp[i11]; sumAlpha += coarseAlpha[i11]; validCount++; }

                    if (validCount == 0)
                    {
                        outPixels[idxPix] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    float temp;
                    byte alpha;

                    if (v00 && v10 && v01 && v11)
                    {
                        float top = Mathf.Lerp(coarseTemp[i00], coarseTemp[i10], tx);
                        float bot = Mathf.Lerp(coarseTemp[i01], coarseTemp[i11], tx);
                        temp = Mathf.Lerp(top, bot, ty);

                        float alphaTop = Mathf.Lerp(coarseAlpha[i00], coarseAlpha[i10], tx);
                        float alphaBot = Mathf.Lerp(coarseAlpha[i01], coarseAlpha[i11], tx);
                        alpha = (byte)Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(alphaTop, alphaBot, ty)), 0, 255);
                    }
                    else
                    {
                        temp = sumTemp / validCount;
                        alpha = (byte)(sumAlpha / validCount);
                    }

                    outPixels[idxPix] = TempToColor(temp, drawer.tempMin, drawer.tempMax, alpha);
                }
            }
        }

        private static bool TryAccumulateNeighborBuckets(
            CsrSpatialIndex spatialIndex,
            int[] xs,
            int[] ys,
            float[] vals,
            int x,
            int y,
            int cx,
            int cy,
            int rangeCells,
            float maxR2,
            int k,
            float[] bestD2,
            int[] bestI,
            ref int bestCount,
            out float exactValue
        )
        {
            exactValue = 0f;

            int gridCols = spatialIndex.gridCols;
            int gridRows = spatialIndex.gridRows;
            int minY = Mathf.Max(0, cy - rangeCells);
            int maxY = Mathf.Min(gridRows - 1, cy + rangeCells);
            int minX = Mathf.Max(0, cx - rangeCells);
            int maxX = Mathf.Min(gridCols - 1, cx + rangeCells);

            for (int ny = minY; ny <= maxY; ny++)
            {
                int rowOffset = ny * gridCols;

                for (int nx = minX; nx <= maxX; nx++)
                {
                    int cellId = rowOffset + nx;
                    int start = spatialIndex.bucketStart[cellId];
                    int end = spatialIndex.bucketStart[cellId + 1];
                    if (start == end) continue;

                    if (TryAccumulateBucketCandidates(
                        spatialIndex.bucketItems,
                        start,
                        end,
                        xs,
                        ys,
                        vals,
                        x,
                        y,
                        maxR2,
                        k,
                        bestD2,
                        bestI,
                        ref bestCount,
                        out exactValue))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryAccumulateBucketCandidates(
            int[] bucketItems,
            int start,
            int end,
            int[] xs,
            int[] ys,
            float[] vals,
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

            for (int ii = start; ii < end; ii++)
            {
                int pi = bucketItems[ii];

                float dx = xs[pi] - x;
                float dy = ys[pi] - y;
                float d2 = dx * dx + dy * dy;

                if (d2 > maxR2)
                    continue;

                if (d2 < ExactHitEpsilon)
                {
                    exactValue = vals[pi];
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

        private static float ComputeIdwTemperature(
            float[] vals,
            float[] bestD2,
            int[] bestI,
            int bestCount,
            float pwr
        )
        {
            double wSum = 0.0;
            double tSum = 0.0;

            bool pwrIs2 = Mathf.Abs(pwr - 2f) < 1e-6f;

            for (int bi = 0; bi < bestCount; bi++)
            {
                double w;
                if (pwrIs2)
                {
                    w = 1.0 / (bestD2[bi] + WeightEpsilon);
                }
                else
                {
                    float d = Mathf.Sqrt(bestD2[bi]);
                    w = 1.0 / (Math.Pow(d, pwr) + WeightEpsilon);
                }

                tSum += vals[bestI[bi]] * w;
                wSum += w;
            }

            return (float)(tSum / wSum);
        }

        private static Color32 TempToColor(float temp, float tMin, float tMax, byte alpha)
        {
            float a = (temp - tMin) / (tMax - tMin + 1e-9f);
            a = Mathf.Clamp01(a);

            byte r, g, b;

            if (a < 0.33f)
            {
                float kk = a / 0.33f;
                r = 0;
                g = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * kk), 0, 255);
                b = 255;
            }
            else if (a < 0.66f)
            {
                float kk = (a - 0.33f) / 0.33f;
                r = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * kk), 0, 255);
                g = 255;
                b = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * (1f - kk)), 0, 255);
            }
            else
            {
                float kk = (a - 0.66f) / 0.34f;
                r = 255;
                g = (byte)Mathf.Clamp(Mathf.RoundToInt(255f * (1f - kk)), 0, 255);
                b = 0;
            }

            return new Color32(r, g, b, alpha);
        }
    }
}
