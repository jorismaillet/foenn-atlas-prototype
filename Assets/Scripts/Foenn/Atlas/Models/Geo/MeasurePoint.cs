using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Models.Geo
{
    public class MeasurePoint
    {
        public float lon, lat, value;

        public MeasurePoint(float lon, float lat, float value)
        {
            this.lon = lon;
            this.lat = lat;
            this.value = value;
        }
    }
}
