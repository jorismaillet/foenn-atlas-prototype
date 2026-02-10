using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public class MetricGroup
    {
        public string name;
        public List<WeatherHistoryMetricKey> keys;

        public MetricGroup(string name, params WeatherHistoryMetricKey[] keys)
        {
            this.name = name;
            this.keys = keys.ToList();
        }
    }
}