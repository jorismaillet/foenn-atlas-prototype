namespace Assets.Scripts.Foenn.Engine.SQL.Dialects
{
    public sealed class SqliteDialect : ISqlDialect
    {
        public string QuoteIdent(string ident) => "\"" + ident.Replace("\"", "\"\"") + "\"";
        public string RenderLimit(int limit) => "LIMIT " + limit;
    }
}