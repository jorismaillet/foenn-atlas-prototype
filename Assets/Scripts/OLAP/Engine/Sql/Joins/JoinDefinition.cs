using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Sql.Joins
{
    public class JoinDefinition
    {
        public Field leftField, rightField;

        public JoinType joinType;

        public JoinDefinition(Field leftField, Field rightField, JoinType joinType)
        {
            this.leftField = leftField;
            this.rightField = rightField;
            this.joinType = joinType;
        }
    }
}
