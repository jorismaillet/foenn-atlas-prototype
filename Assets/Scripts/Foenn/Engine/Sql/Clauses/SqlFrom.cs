using Assets.Scripts.Foenn.Engine.Sql.Dialects;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlFrom
    {
        public readonly string clause;
        public SqlFrom(string table, ISqlDialect dialect)
        {
            clause = $" FROM {dialect.QuoteIdent(table)}";
        }
    }
}