namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    using Assets.Scripts.Foenn.Datasets;

    public class SqlFrom
    {
        public readonly string clause;

        public SqlFrom(ITable table)
        {
            clause = $" FROM \"{table.Name}\"";
        }
    }
}
