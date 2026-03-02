using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public readonly struct HeatmapSettings
    {
        public readonly RenderSettings render;

        public readonly float idwPower;       // typically 2
        public readonly int maxNeighbors;     // e.g. 16
        public readonly float maxRadiusPx;    // e.g. 80..150 (depends resolution & density)

        public readonly float alpha;          // 0..1 overlay alpha
        public readonly float tempMin;        // for color mapping
        public readonly float tempMax;        // for color mapping

        // Divides the grid into computing cells. Used for spatial hashing and interpolation
        public readonly int cellSizePx;       // e.g. 32

        public HeatmapSettings(RenderSettings render, float idwPower, int maxNeighbors, float maxRadiusPx, float alpha, float tempMin, float tempMax, int cellSizePx)
        {
            this.render = render;
            this.idwPower = idwPower;
            this.maxNeighbors = maxNeighbors;
            this.maxRadiusPx = maxRadiusPx;
            this.alpha = alpha;
            this.tempMin = tempMin;
            this.tempMax = tempMax;
            this.cellSizePx = cellSizePx;
        }

        public void Validate()
        {
            render.Validate();

            if (cellSizePx <= 0)
                throw new ArgumentException("Invalid cell size.");

            if (idwPower <= 0f)
                throw new ArgumentException("Invalid idwPower.");

            if (maxNeighbors <= 0)
                throw new ArgumentException("Invalid maxNeighbors.");

            if (maxRadiusPx <= 0f)
                throw new ArgumentException("Invalid maxRadiusPx.");

            if (alpha < 0f || alpha > 1f)
                throw new ArgumentException("Invalid alpha.");

            if (tempMax <= tempMin)
                throw new ArgumentException("Invalid temperature range.");

        }
    }
}
