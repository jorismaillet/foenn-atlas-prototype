using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Sql.Filters
{
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
