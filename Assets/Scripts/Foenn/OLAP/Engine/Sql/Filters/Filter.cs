namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Schema;

    public abstract class Filter
    {
        public Field filteredField;

        protected Filter(Field filteredField)
        {
            this.filteredField = filteredField;
        }

        public abstract string ToSql();
    }
}
