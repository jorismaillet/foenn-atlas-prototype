using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public class JoinDefinition
    {
        public PrefixedField leftColumn, rightColumn;
        public JoinType joinType;

        public JoinDefinition(PrefixedField leftColumn, PrefixedField rightColumn, JoinType joinType) {
            this.leftColumn = leftColumn;
            this.rightColumn = rightColumn;
            this.joinType = joinType;
        }
    }
}
