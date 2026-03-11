namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.OLAP.Schema;
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
