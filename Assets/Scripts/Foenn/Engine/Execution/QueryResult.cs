using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryResult
    {
        public List<string> rawHeaders;
        public List<Dimension> dimensions;

        public QueryResult(List<string> rawHeaders, List<List<string>> rawResult)
        {
            this.rawHeaders = rawHeaders;
            dimensions = new List<Dimension>();
            foreach (var row in rawResult)
            {
                var dim = new Dimension
                {
                    attributeValues = new List<Assets.Scripts.Foenn.Engine.Attributes.AttributeValue>(),
                    metricValues = new List<Assets.Scripts.Foenn.Engine.Metrics.MetricValue>()
                };
                for (int i = 0; i < rawHeaders.Count && i < row.Count; i++)
                {
                    var header = rawHeaders[i];
                    var value = row[i];
                    // Heuristique : si le header est un MetricKey, c'est une métrique
                    if (System.Enum.TryParse<Assets.Scripts.Foenn.Engine.Metrics.MetricKey>(header, out var metricKey))
                    {
                        if (float.TryParse(value, out var fval))
                        {
                            dim.metricValues.Add(new Assets.Scripts.Foenn.Engine.Metrics.MetricValue { metric = null, value = fval });
                        }
                    }
                    else
                    {
                        dim.attributeValues.Add(new Assets.Scripts.Foenn.Engine.Attributes.AttributeValue { Attribute = null, value = value });
                    }
                }
                dimensions.Add(dim);
            }
        }
    }
}