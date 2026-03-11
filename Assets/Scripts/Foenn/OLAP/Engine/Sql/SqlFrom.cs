namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Schema;

    public class SqlFrom
    {
        public readonly string clause;

        public SqlFrom(ITable table)
        {
            clause = $" FROM \"{table.Name}\"";
        }
    }
}
