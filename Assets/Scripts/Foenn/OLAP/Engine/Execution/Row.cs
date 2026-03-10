using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class Row
    {
        public TimeField time;
        public GeoField geo;
        public Dictionary<PrefixedField, object> values = new Dictionary<PrefixedField, object>();
    }
}