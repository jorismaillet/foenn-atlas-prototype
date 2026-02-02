using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Metrics;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryRequest
    {
        public List<Metric> metrics;
        public List<Attribute> attributes;
        public List<Filter> Filters;
    }
}