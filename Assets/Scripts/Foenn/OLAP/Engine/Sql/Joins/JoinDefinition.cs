using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public class JoinDefinition
    {
        public IDataField leftField, rightField;
        public JoinType joinType;

        public JoinDefinition(IDataField leftField, IDataField rightField, JoinType joinType) {
            this.leftField = leftField;
            this.rightField = rightField;
            this.joinType = joinType;
        }
    }
}
