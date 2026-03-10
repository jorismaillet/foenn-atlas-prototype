using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    public class MetricGroup
    {
        public string name;
        public List<Field> metrics;

        public MetricGroup(string name, params Field[] metrics)
        {
            this.name = name;
            this.metrics = new List<Field>(metrics);
        }
    }
}