using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.OLAP;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
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
            rows = new List<Row>();
            foreach (var rawRow in rawResult)
            {
                var row = new Row();
                for (int i = 0; i < rawHeaders.Count && i < rawRow.Count; i++)
                {
                    var header = rawHeaders[i];
                    string value = rawRow[i];
                    // Heuristique : si le header est un MetricKey, c'est une métrique
                    if (System.Enum.TryParse<MetricKey>(header, out var metricKey))
                    {
                        if (float.TryParse(value, out var fval))
                        {
                            var metrics = request.metrics.Find(m => m.key == metricKey);
                            row.measures.Add(new Measure(metrics, fval));
                        }
                    }
                    else if (System.Enum.TryParse<AttributeKey>(header, out var attributeKey))
                    {
                        row.dimensions.Add(new TextDimension(new AttributeValue(new Attribute(attributeKey, Attribute.Name(attributeKey)), value)));
                    }
                    else
                    {
                        throw new System.Exception($"Unknown header key: {header}");
                    }
                }
                rows.Add(row);
            }
        }
    }
}