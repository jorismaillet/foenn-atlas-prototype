using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine
{
    public class QueryResult
    {
        public List<Row> rows;
        private List<Field> columns;
        private Func<object[], GeoPoint> getGeo;

        public QueryResult(List<Field> columns)
        {
            this.rows = new List<Row>();
            this.columns = columns;
            Dictionary<string, int> geoIndexes = new Dictionary<string, int>();
            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].analyticsType.Equals(AnalyticsType.GEO_LAT))
                {
                    geoIndexes["lat"] = i;
                }
                else if(columns[i].analyticsType.Equals(AnalyticsType.GEO_LON)) {
                    geoIndexes["lon"] = i;
                }
            }
            if (geoIndexes.Count == 2)
            {
                AddGeoParser(geoIndexes);
            }
        }

        public void ParseLine(object[] values)
        {
            Row row = new Row();
            for (int i = 0; i < columns.Count; i++)
            {
                row.values[columns[i]] = values[i];
            }
            if (getGeo != null)
            {
                row.geo = getGeo(values);
            }
            rows.Add(row);
        }

        private void AddGeoParser(Dictionary<string, int> geoIndexes)
        {
            getGeo = (object[] rawLine) =>
            {
                return new GeoPoint(
                    (float)rawLine[geoIndexes["lat"]],
                    (float)rawLine[geoIndexes["lon"]]
                );
            };
        }
    }
}
