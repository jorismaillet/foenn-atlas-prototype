using Assets.Scripts.Foenn.Engine.Sql.Dialects;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlFrom
    {
        private readonly string fromClause;
        public SqlFrom(string table, ISqlDialect dialect)
        {
            fromClause = $" FROM {dialect.QuoteIdent(table)}";
        }
        public override string ToString() => fromClause;
    }
}