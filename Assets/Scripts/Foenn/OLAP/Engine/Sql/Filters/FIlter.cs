namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    using Assets.Scripts.Foenn.OLAP.Engine.Sql;

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
