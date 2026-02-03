using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlWhere
    {
        private readonly string whereClause;

        public SqlWhere(QueryRequest request, ISqlDialect dialect, SqlEmit emit)
        {
            var whereParts = new List<string>();
            foreach (var filter in request.filters)
            {
                // TODO: Adapter selon le type de filtre (ici exemple simple)
                if (filter is DataFilter df)
                {
                    var paramName = emit.AddParam(string.Join(", ", df.selectedValues));
                    whereParts.Add($"{dialect.QuoteIdent(df.filteredAttributeKey.ToString())} {df.mode} {paramName}");
                }
                // Ajouter d'autres types de filtres ici (TimeFilter, GeoFilter, etc.)
            }
            if (whereParts.Count > 0)
                whereClause = " WHERE " + string.Join(" AND ", whereParts);
            else
                whereClause = string.Empty;
        }
        public override string ToString() => whereClause;
    }
}