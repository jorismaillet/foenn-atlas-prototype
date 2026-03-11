namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Schema;

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
