namespace Assets.Scripts.Foenn.Atlas.Models.Activities
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    /// <summary>
    /// Frontend business model representing a group of metrics for activity conditions.
    /// </summary>
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
