using System;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public readonly struct HeatmapSettings
    {
        public readonly float idwPower;       // typically 2
        public readonly int maxNeighbors;     // e.g. 16
        public readonly float maxRadiusPx;    // e.g. 80..150 (depends resolution & density)

        // Divides the grid into computing cells. Used for spatial hashing and interpolation
        public readonly int cellSizePx;       // e.g. 32

        public HeatmapSettings(float idwPower, int maxNeighbors, float maxRadiusPx, int cellSizePx)
        {
            this.idwPower = idwPower;
            this.maxNeighbors = maxNeighbors;
            this.maxRadiusPx = maxRadiusPx;
            this.cellSizePx = cellSizePx;
        }

        public void Validate()
        {

            if (cellSizePx <= 0)
                throw new ArgumentException("Invalid cell size.");

            if (idwPower <= 0f)
                throw new ArgumentException("Invalid idwPower.");

            if (maxNeighbors <= 0)
                throw new ArgumentException("Invalid maxNeighbors.");

            if (maxRadiusPx <= 0f)
                throw new ArgumentException("Invalid maxRadiusPx.");

        }
    }
}
