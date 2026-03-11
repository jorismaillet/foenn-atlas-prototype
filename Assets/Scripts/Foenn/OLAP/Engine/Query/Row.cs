namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.OLAP.Fields;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    public class Row
    {
        public TimeField time;

        public GeoField geo;

        public Dictionary<IDataField, object> values = new Dictionary<IDataField, object>();
    }
}
