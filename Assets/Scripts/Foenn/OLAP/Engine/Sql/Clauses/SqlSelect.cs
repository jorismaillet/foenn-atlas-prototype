using Assets.Scripts.Foenn.Datasets;
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

        public SqlSelect(List<ITable> tables, List<PrefixedField> selectedColumns) {
            var selected = tables.Select(t => $"\"{t.Name}\".*");
            selected.Concat(selectedColumns.Select(c => c.ToSql()));
            clause = "SELECT " + string.Join(", ", selected);
        }
    }
}