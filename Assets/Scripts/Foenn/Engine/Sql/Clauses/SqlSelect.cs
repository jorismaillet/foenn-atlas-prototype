using Assets.Scripts.Foenn.Engine.OLAP;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlSelect
    {
        public readonly string clause;
        public SqlSelect(List<Metric> selectedMetrics, List<Attribute> selectedAttributes)
        {
            var selectParts = SelectedMetrics(selectedMetrics).Concat(SelectedAttributes(selectedAttributes)).ToList();
            clause = "SELECT " + (selectParts.Any() ? string.Join(", ", selectParts) : "*");
        }
        public IEnumerable<string> SelectedMetrics(List<Metric> selectedMetrics)
        {
            return selectedMetrics.Select(m =>
            {
                if (m.aggregation == null)
                {
                    return $"\"{m.key}\"";
                }
                else
                {
                    return $"{m.aggregation}(\"{m.key}\")";
                }
            });
        }
        public IEnumerable<string> SelectedAttributes(List<Attribute> selectedAttributes)
        {
            return selectedAttributes.Select(a => $"\"{a.key}\"");
        }
    }
}