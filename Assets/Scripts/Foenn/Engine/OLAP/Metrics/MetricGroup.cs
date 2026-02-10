using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
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