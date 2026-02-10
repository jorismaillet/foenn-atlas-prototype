using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlSortBy
    {
        public List<Attribute> attributes;
        public SortOrder order;

        public SqlSortBy(SortOrder order, params Attribute[] attributes)
        {
            this.attributes = attributes.ToList();
            this.order = order;
        }
    }
}