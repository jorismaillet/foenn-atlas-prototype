namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Schema;

    public abstract class Filter
    {
        public IDataField filteredField;

        protected Filter(IDataField filteredField)
        {
            this.filteredField = filteredField;
        }

        public abstract string ToSql();
    }
}
