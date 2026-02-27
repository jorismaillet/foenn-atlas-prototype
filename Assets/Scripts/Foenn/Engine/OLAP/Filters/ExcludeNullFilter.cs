using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class ExcludeNullFilter : Filter
    {
        public WeatherHistoryMetricKey metricKey;

        public ExcludeNullFilter(WeatherHistoryMetricKey metricKey) : base()
        {
            this.metricKey = metricKey;
        }
    }
}