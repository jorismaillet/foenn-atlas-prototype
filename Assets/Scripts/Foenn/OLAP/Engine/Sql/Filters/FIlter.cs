using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public abstract class Filter
    {
        public PrefixedField column;

        protected Filter(PrefixedField column) {
            this.column = column;
        }

        public abstract string ToSql();
    }
}