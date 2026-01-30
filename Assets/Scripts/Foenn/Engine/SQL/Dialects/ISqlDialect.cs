namespace Assets.Scripts.Foenn.Engine.SQL
{
    public interface ISqlDialect
    {
        string QuoteIdent(string ident);
        string RenderLimit(int limit);
    }
}