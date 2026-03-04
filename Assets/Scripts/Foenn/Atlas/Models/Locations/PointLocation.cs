using Assets.Scripts.Foenn.Atlas.Models.Geo;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Models.Locations
{
    public class PointLocation : GeoPoint, ILocation
    {
        public string name;

        public PointLocation(string name, float lat, float lon) : base(lat, lon)
        {
            this.name = name;
        }

        public string Name => this.name;
    }
}
