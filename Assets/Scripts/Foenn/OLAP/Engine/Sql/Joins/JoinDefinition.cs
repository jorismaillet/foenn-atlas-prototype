namespace Assets.Scripts.Foenn.Engine.Sql
{
    using Assets.Scripts.Foenn.Engine.Sql.Clauses;
    using Assets.Scripts.Foenn.OLAP.Engine.Sql;

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
