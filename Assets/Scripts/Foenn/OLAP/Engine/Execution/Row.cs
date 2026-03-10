using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class Row
    {
        public TimeField time;
        public GeoField geo;
        public Dictionary<IDataField, object> values = new Dictionary<IDataField, object>();
    }
}