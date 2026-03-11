namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;
    using System.Linq;

    public class SqlSelect
    {
        public readonly string clause;

        public SqlSelect(List<ITable> tables, List<Field> selectedColumns)
        {
            var selected = tables.Select(t => $"\"{t.TableName}\".*");
            selected.Concat(selectedColumns.Select(c => c.ToSql()));
            clause = "SELECT " + string.Join(", ", selected);
        }
    }
}
