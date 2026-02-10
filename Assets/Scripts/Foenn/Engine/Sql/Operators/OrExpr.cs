using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Sql.Operators
{
    public sealed class OrExpr : FilterExpr
    {
        public OrExpr(IReadOnlyList<FilterExpr> terms) { Terms = terms; }
        public IReadOnlyList<FilterExpr> Terms { get; }
    }
}