namespace Assets.Scripts.Foenn.Engine.Sql.Dialects
{
    public sealed class SqliteDialect : ISqlDialect
    {
        public string QuoteIdent(string ident) => "\"" + ident.Replace("\"", "\"\"") + "\"";
        public string RenderLimit(int limit) => "LIMIT " + limit;
    }
}