using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public abstract class Filter
    {
        public IDataField filteredField;

        protected Filter(IDataField filteredField) {
            this.filteredField = filteredField;
        }

        public abstract string ToSql();
    }
}