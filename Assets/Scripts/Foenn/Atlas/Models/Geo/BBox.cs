using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public class BBox
    {
        public static BBox France = new BBox(-5.5F, 41.0F, 20.0F, 51.5F);

        public float minLon, minLat, maxLon, maxLat;

        public BBox(float minLon, float minLat, float maxLon, float maxLat)
        {
            this.minLon = minLon;
            this.minLat = minLat;
            this.maxLon = maxLon;
            this.maxLat = maxLat;
        }
    }
}
