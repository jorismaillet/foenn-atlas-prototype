using System.Collections.Generic;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Result
{
    public class Row
    {
        public GeoPoint geo;

        public Dictionary<Field, object> values = new Dictionary<Field, object>();
    }
}
