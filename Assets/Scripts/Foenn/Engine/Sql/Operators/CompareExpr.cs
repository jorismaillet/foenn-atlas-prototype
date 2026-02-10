namespace Assets.Scripts.Foenn.Engine.Sql.Operators
{
    public sealed class CompareExpr : FilterExpr
    {
        public CompareExpr(string fieldRef, CompareOp op, object value)
        {
            FieldRef = fieldRef;
            Op = op;
            Value = value;
        }

        public string FieldRef { get; }
        public CompareOp Op { get; }
        public object Value { get; }
    }
}