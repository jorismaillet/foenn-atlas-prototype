using System;
using System.Collections.Generic;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Engine
{
    public class Row
    {
        public readonly GeoPoint geo;

        public readonly Dictionary<Field, object> values;

        public Row(GeoPoint geo = null)
        {
            this.geo = geo;
            this.values = new Dictionary<Field, object>();
        }

        public int IntValue(Field field)
        {
            return Convert.ToInt32(values[field]);
        }

        public string StringValue(Field field)
        {
            return Convert.ToString(values[field]);
        }

        public float FloatValue(Field field)
        {
            return Convert.ToSingle(values[field]);
        }
    }
}
