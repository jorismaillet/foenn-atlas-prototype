using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    public class HeatmapSettings
    {
        public int width;
        public int height;

        public BBox bBox;

        public float idwPower;       // typically 2
        public int maxNeighbors;     // e.g. 16
        public float maxRadiusPx;    // e.g. 80..150 (depends resolution & density)

        public float alpha;          // 0..1 overlay alpha
        public float tempMin;        // for color mapping
        public float tempMax;        // for color mapping

        // Spatial hash cell size in pixels (bigger -> fewer cells, more candidates)
        public int cellSizePx;       // e.g. 32
    }
}
