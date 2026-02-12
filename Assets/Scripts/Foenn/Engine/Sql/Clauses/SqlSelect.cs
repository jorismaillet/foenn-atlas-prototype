using Assets.Scripts.Foenn.Engine.OLAP;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlSelect
    {
        public readonly string clause;
        public SqlSelect(List<Metric> selectedMetrics, List<Attribute> selectedAttributes, ISqlDialect dialect)
        {
            var selectParts = SelectedMetrics(selectedMetrics, dialect).Concat(SelectedAttributes(selectedAttributes, dialect)).ToList();
            clause = "SELECT " + (selectParts.Any() ? string.Join(", ", selectParts) : "*");
        }
        public IEnumerable<string> SelectedMetrics(List<Metric> selectedMetrics, ISqlDialect dialect)
        {
            return selectedMetrics.Select(m => $"{m.aggregation}({dialect.QuoteIdent(m.key.ToString())})");
        }
        public IEnumerable<string> SelectedAttributes(List<Attribute> selectedAttributes, ISqlDialect dialect)
        {
            return selectedAttributes.Select(a => $"{dialect.QuoteIdent(a.key.ToString())}");
        }
    }
}