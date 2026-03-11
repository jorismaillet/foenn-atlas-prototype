namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Schema;

    public class JoinDefinition
    {
        public IDataField leftField, rightField;

        public JoinType joinType;

        public JoinDefinition(IDataField leftField, IDataField rightField, JoinType joinType)
        {
            this.leftField = leftField;
            this.rightField = rightField;
            this.joinType = joinType;
        }
    }
}
