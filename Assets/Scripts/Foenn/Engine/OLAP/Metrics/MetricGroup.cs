using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public class MetricGroup
    {
        public string name;
        public List<MetricKey> keys;

        public MetricGroup(string name, params MetricKey[] keys)
        {
            this.name = name;
            this.keys = keys.ToList();
        }
    }
}