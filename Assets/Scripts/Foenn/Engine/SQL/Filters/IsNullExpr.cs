namespace Assets.Scripts.Foenn.Engine.Sql.Filters
{
    public sealed class IsNullExpr : FilterExpr
    {
        public IsNullExpr(string fieldRef, bool isNull)
        {
            FieldRef = fieldRef;
            IsNull = isNull;
        }

        public string FieldRef { get; }
        public bool IsNull { get; }
    }
}