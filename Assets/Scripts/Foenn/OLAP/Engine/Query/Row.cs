namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    public class Row
    {
        public Schema.TimeField time;
        public Schema.GeoField geo;
        public Dictionary<IDataField, object> values = new Dictionary<IDataField, object>();
    }
}
