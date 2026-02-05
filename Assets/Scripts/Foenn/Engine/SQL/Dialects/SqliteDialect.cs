namespace Assets.Scripts.Foenn.Engine.Sql.Dialects
{
    public class SqliteDialect : ISqlDialect
    {
        public string Equals() => "=";
        public string Different() => "!=";
        public string EndOfLine() => ";";
        public string QuoteIdent(string ident) => "\"" + ident.Replace("\"", "\"\"") + "\"";
        public string RenderLimit(int limit) => "LIMIT " + limit;
    }
}