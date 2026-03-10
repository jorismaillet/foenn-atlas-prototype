using Assets.Scripts.Foenn.Datasets;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlFrom
    {
        public readonly string clause;
        public SqlFrom(ITable table)
        {
            clause = $" FROM \"{table.Name}\"";
        }
    }
}