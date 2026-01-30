namespace Assets.Scripts.Foenn.Engine.SQL.Filters
{
    public sealed class NotExpr : FilterExpr
    {
        public NotExpr(FilterExpr inner) { Inner = inner; }
        public FilterExpr Inner { get; }
    }
}