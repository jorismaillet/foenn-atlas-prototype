namespace Assets.Scripts.Foenn.Engine.Sql.Operators
{
    public sealed class BetweenExpr : FilterExpr
    {
        public BetweenExpr(string fieldRef, object from, object to)
        {
            FieldRef = fieldRef;
            From = from;
            To = to;
        }

        public string FieldRef { get; }
        public object From { get; }
        public object To { get; }
    }
}