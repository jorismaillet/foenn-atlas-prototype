using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.Models.Activities
{
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
