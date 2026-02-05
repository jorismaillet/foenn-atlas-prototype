using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.OLAP;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Times;
using Assets.Scripts.Foenn.Utils;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryResult
    {
        public List<string> rawHeaders;
        public List<Row> rows;

        public QueryResult(QueryRequest request, List<string> rawHeaders, List<List<string>> rawResult)
        {
            this.rawHeaders = rawHeaders;
            this.rows = Parse(request, rawHeaders, rawResult);
        }

        public static List<Row> Parse(QueryRequest request, List<string> rawHeaders, List<List<string>> rawResult)
        {
            var rows = new List<Row>();
            foreach (var rawRow in rawResult)
            {
                var row = new Row();
                for (int i = 0; i < rawHeaders.Count && i < rawRow.Count; i++)
                {
                    var header = rawHeaders[i];
                    string value = rawRow[i];
                    if (System.Enum.TryParse<MetricKey>(header, out var metricKey))
                    {
                        if (float.TryParse(value, out var fval))
                        {
                            var metrics = request.selectedMetrics.Find(m => m.key == metricKey);
                            row.measures.Add(new Measure(metrics, fval));
                        }
                    }
                    else if (System.Enum.TryParse<TimeAttributeKey>(header, out var timeKey))
                    {
                        row.time = new TimeDimension();
                        row.time.start = TimeUtils.Date(value);
                    }
                    else if (System.Enum.TryParse<GeoAttributeKey>(header, out var geoKey))
                    {
                        row.geo = new GeoDimension();
                        row.geo.numPost = value;
                    }
                    else if (System.Enum.TryParse<AttributeKey>(header, out var attributeKey))
                    {
                        row.attributes.Add(new Attribute(attributeKey, value));
                    }
                    else
                    {
                        throw new System.Exception($"Unknown header key: {header}");
                    }
                }
                rows.Add(row);
            }
            return rows;
        }
    }
}