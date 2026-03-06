#if UNITY_2019_1_OR_NEWER
using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
#if ENABLE_BURST_COMPILATION
using Unity.Burst;
#endif

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    internal static class HeatmapDrawerJobs
    {
        public static bool TryComputeCoarseField(
            int[] xs,
            int[] ys,
            float[] vals,
            CsrSpatialIndex spatialIndex,
            HeatmapSettings settings,
            int cellSize,
            float maxR2,
            float pwr,
            int k,
            int rangeCells,
            byte baseAlpha,
            int w,
            int h,
            int coarseW,
            int coarseH,
            int sampleStep,
            float[] coarseTemp,
            byte[] coarseAlpha)
        {
            // In editor/player this is expected to exist, but keep the path best-effort.
            // If Unity.Collections/Jobs are not available, this file is excluded by preprocessor.
            if (xs == null || ys == null || vals == null || coarseTemp == null || coarseAlpha == null)
                return false;

            if (xs.Length != ys.Length || xs.Length != vals.Length)
                return false;

            int coarseCount = coarseW * coarseH;
            if (coarseTemp.Length != coarseCount || coarseAlpha.Length != coarseCount)
                return false;

            // Copy managed arrays into NativeArrays (TempJob).
            // For maximum performance, make these persistent & reuse between frames.
            var nxs = new NativeArray<int>(xs, Allocator.TempJob);
            var nys = new NativeArray<int>(ys, Allocator.TempJob);
            var nvals = new NativeArray<float>(vals, Allocator.TempJob);
            var nbucketStart = new NativeArray<int>(spatialIndex.bucketStart, Allocator.TempJob);
            var nbucketItems = new NativeArray<int>(spatialIndex.bucketItems, Allocator.TempJob);

            var nTemp = new NativeArray<float>(coarseCount, Allocator.TempJob);
            var nAlpha = new NativeArray<byte>(coarseCount, Allocator.TempJob);

            try
            {
                var job = new CoarseFieldJob
                {
                    xs = nxs,
                    ys = nys,
                    vals = nvals,
                    bucketStart = nbucketStart,
                    bucketItems = nbucketItems,

                    gridCols = spatialIndex.gridCols,
                    gridRows = spatialIndex.gridRows,

                    cellSize = Mathf.Max(1, cellSize),
                    w = w,
                    h = h,
                    coarseW = coarseW,
                    coarseH = coarseH,
                    sampleStep = Mathf.Max(1, sampleStep),

                    maxR2 = maxR2,
                    maxRadiusPx = settings.maxRadiusPx,
                    pwr = pwr,
                    // Note: this Jobs path currently uses all candidates within radius (not top-K),
                    // so k is kept for future parity but isn't used in the job.
                    k = Mathf.Clamp(k, 1, 32),
                    rangeCells = Mathf.Max(0, rangeCells),
                    baseAlpha = baseAlpha,

                    outTemp = nTemp,
                    outAlpha = nAlpha
                };

                // Small batch size keeps work balanced (coarse grids are usually <= 256^2).
                job.Schedule(coarseCount, 64).Complete();

                nTemp.CopyTo(coarseTemp);
                nAlpha.CopyTo(coarseAlpha);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                nTemp.Dispose();
                nAlpha.Dispose();
                nxs.Dispose();
                nys.Dispose();
                nvals.Dispose();
                nbucketStart.Dispose();
                nbucketItems.Dispose();
            }
        }

#if ENABLE_BURST_COMPILATION
        [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
#endif
        private struct CoarseFieldJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<int> xs;
            [ReadOnly] public NativeArray<int> ys;
            [ReadOnly] public NativeArray<float> vals;

            [ReadOnly] public NativeArray<int> bucketStart;
            [ReadOnly] public NativeArray<int> bucketItems;

            public int gridCols;
            public int gridRows;

            public int cellSize;
            public int w;
            public int h;
            public int coarseW;
            public int coarseH;
            public int sampleStep;

            public float maxR2;
            public float maxRadiusPx;
            public float pwr;
            public int k;
            public int rangeCells;
            public byte baseAlpha;

            [WriteOnly] public NativeArray<float> outTemp;
            [WriteOnly] public NativeArray<byte> outAlpha;

            public void Execute(int index)
            {
                int gx = index % coarseW;
                int gy = index / coarseW;

                int x = gx * sampleStep;
                int y = gy * sampleStep;
                if (x >= w) x = w - 1;
                if (y >= h) y = h - 1;

                int cx = x / cellSize;
                int cy = y / cellSize;

                float exact = 0f;
                bool exactHit = false;

                double wSum = 0.0;
                double tSum = 0.0;
                float nearestD2 = float.PositiveInfinity;

                bool pwrIs2 = Mathf.Abs(pwr - 2f) < 1e-6f;

                int minY = cy - rangeCells;
                if (minY < 0) minY = 0;
                int maxY = cy + rangeCells;
                if (maxY >= gridRows) maxY = gridRows - 1;
                int minX = cx - rangeCells;
                if (minX < 0) minX = 0;
                int maxX = cx + rangeCells;
                if (maxX >= gridCols) maxX = gridCols - 1;

                for (int ny = minY; ny <= maxY && !exactHit; ny++)
                {
                    int rowOffset = ny * gridCols;
                    for (int nx = minX; nx <= maxX && !exactHit; nx++)
                    {
                        int cellId = rowOffset + nx;
                        int start = bucketStart[cellId];
                        int end = bucketStart[cellId + 1];

                        for (int ii = start; ii < end; ii++)
                        {
                            int pi = bucketItems[ii];

                            float dx = xs[pi] - x;
                            float dy = ys[pi] - y;
                            float d2 = dx * dx + dy * dy;

                            if (d2 > maxR2) continue;

                            if (d2 < 1e-6f)
                            {
                                exact = vals[pi];
                                exactHit = true;
                                break;
                            }

                            if (d2 < nearestD2)
                                nearestD2 = d2;

                            double wgt;
                            if (pwrIs2)
                            {
                                wgt = 1.0 / (d2 + 1e-6);
                            }
                            else
                            {
                                float d = Mathf.Sqrt(d2);
                                wgt = 1.0 / (Math.Pow(d, pwr) + 1e-6);
                            }

                            tSum += vals[pi] * wgt;
                            wSum += wgt;
                        }
                    }
                }

                if (exactHit)
                {
                    outTemp[index] = exact;
                    outAlpha[index] = baseAlpha;
                    return;
                }

                if (wSum <= 0.0 || float.IsPositiveInfinity(nearestD2))
                {
                    outTemp[index] = float.NaN;
                    outAlpha[index] = 0;
                    return;
                }

                float temp = (float)(tSum / wSum);
                float nearestD = Mathf.Sqrt(nearestD2);
                float fade = 1f - (nearestD / (maxRadiusPx + 1e-6f));
                if (fade < 0f) fade = 0f;
                if (fade > 1f) fade = 1f;

                byte alpha = (byte)Mathf.Clamp(Mathf.RoundToInt(baseAlpha * fade), 0, 255);

                outTemp[index] = temp;
                outAlpha[index] = alpha;
            }
        }
    }
}
#endif
