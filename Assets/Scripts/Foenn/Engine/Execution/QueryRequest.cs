using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Metrics;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryRequest
    {
        public List<Metric> metrics = new List<Metric>();
        public List<Attribute> attributes = new List<Attribute>();
        public List<Filter> filters = new List<Filter>();

        public QueryRequest Select(params Metric[] metrics)
        {
            this.metrics.AddRange(metrics);
            return this;
        }

        public QueryRequest GroupBy(params Attribute[] attributes)
        {
            this.attributes.AddRange(attributes);
            return this;
        }

        public QueryRequest Where(params Filter[] filters)
        {
            this.filters.AddRange(filters);
            return this;
        }
    }
}