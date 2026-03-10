namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    using Assets.Scripts.Foenn.ETL.Models;
    using System.Collections.Generic;

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
