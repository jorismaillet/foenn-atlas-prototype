using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Sql.Filters
{
    public sealed class AndExpr : FilterExpr
    {
        public AndExpr(IReadOnlyList<FilterExpr> terms) { Terms = terms; }
        public IReadOnlyList<FilterExpr> Terms { get; }
    }
}