using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Sql.Clauses
{
    public class SqlSelect
    {
        public readonly string clause;

        public SqlSelect(List<ITable> tables, List<Field> selectedColumns)
        {
            var selected = tables.Select(t => $"\"{t.TableName}\".*");
            selected.Concat(selectedColumns.Select(c => c.ToSql()));
            var statement = selected.Any() ? string.Join(", ", selected) : "*";
            clause = "SELECT " + statement;
        }
    }
}
