using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Sql.Filters
{

    public sealed class InExpr : FilterExpr
    {
        public InExpr(string fieldRef, IReadOnlyList<object> values)
        {
            FieldRef = fieldRef;
            Values = values;
        }

        public string FieldRef { get; }
        public IReadOnlyList<object> Values { get; }
    }
}