namespace Assets.Scripts.Foenn.Engine.Sql.Dialects
{
    public interface ISqlDialect
    {
        string QuoteIdent(string ident);
        string RenderLimit(int limit);
        string Equals();
        string Different();
        string EndOfLine();
    }
}