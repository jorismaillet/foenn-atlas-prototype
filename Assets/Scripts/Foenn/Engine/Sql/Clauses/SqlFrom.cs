namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlFrom
    {
        public readonly string clause;
        public SqlFrom(string table)
        {
            clause = $" FROM \"{table}\"";
        }
    }
}