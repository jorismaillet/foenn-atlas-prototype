using System.Collections.Generic;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine
{
    public class Row
    {
        public GeoPoint geo;

        public Dictionary<Field, object> values = new Dictionary<Field, object>();

        public int IntValue(Field field)
        {
            return (int)values[field];
        }
        public string StringValue(Field field)
        {
            return (string)values[field];
        }
        public float FloatValue(Field field)
        {
            return (float)values[field];
        }
    }
}
