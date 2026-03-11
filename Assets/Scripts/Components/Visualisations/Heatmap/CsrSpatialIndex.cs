namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    using System;

    // Compact CSR-like spatial index for a uniform grid.
    // bucketStart has length (gridCols*gridRows + 1) and bucketItems contains measure indices.
    public readonly struct CsrSpatialIndex
    {
        public readonly int gridCols;

        public readonly int gridRows;

        public readonly int cellCount;

        public readonly int[] bucketStart;

        public readonly int[] bucketItems;

        public CsrSpatialIndex(int gridCols, int gridRows, int[] bucketStart, int[] bucketItems)
        {
            this.gridCols = gridCols;
            this.gridRows = gridRows;
            this.cellCount = gridCols * gridRows;
            this.bucketStart = bucketStart;
            this.bucketItems = bucketItems;
        }

        public static CsrSpatialIndex Build(int[] xs, int[] ys, int cellSize, int gridCols, int gridRows)
        {
            if (xs == null) throw new ArgumentNullException(nameof(xs));
            if (ys == null) throw new ArgumentNullException(nameof(ys));
            if (xs.Length != ys.Length) throw new ArgumentException("xs and ys must have the same length.");

            int cellCount = gridCols * gridRows;
            var counts = new int[cellCount];

            int n = xs.Length;
            for (int i = 0; i < n; i++)
            {
                int cx = xs[i] / cellSize;
                int cy = ys[i] / cellSize;
                if (cx < 0 || cx >= gridCols || cy < 0 || cy >= gridRows)
                    continue;

                counts[cy * gridCols + cx]++;
            }

            var bucketStart = new int[cellCount + 1];
            int sum = 0;
            for (int c = 0; c < cellCount; c++)
            {
                bucketStart[c] = sum;
                sum += counts[c];
            }
            bucketStart[cellCount] = sum;

            var bucketItems = new int[sum];
            Array.Clear(counts, 0, counts.Length);

            for (int i = 0; i < n; i++)
            {
                int cx = xs[i] / cellSize;
                int cy = ys[i] / cellSize;
                if (cx < 0 || cx >= gridCols || cy < 0 || cy >= gridRows)
                    continue;

                int cellId = cy * gridCols + cx;
                int write = bucketStart[cellId] + counts[cellId]++;
                bucketItems[write] = i;
            }

            return new CsrSpatialIndex(gridCols, gridRows, bucketStart, bucketItems);
        }
    }
}
