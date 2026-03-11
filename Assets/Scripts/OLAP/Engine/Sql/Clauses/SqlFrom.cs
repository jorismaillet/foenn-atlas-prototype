using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Sql.Clauses
{
    public class SqlFrom
    {
        public readonly string clause;

        public SqlFrom(ITable table)
        {
            clause = $" FROM \"{table.TableName}\"";
        }
    }
}
