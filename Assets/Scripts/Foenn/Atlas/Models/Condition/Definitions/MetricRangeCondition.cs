using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions
{
    public class MetricRangeCondition : ICondition
    {
        public WeatherHistoryMetricKey metricKey;
        public float min;
        public float max;

        public MetricRangeCondition(WeatherHistoryMetricKey metricKey, float min, float max)
        {
            this.metricKey = metricKey;
            this.min = min;
            this.max = max;
        }

        public bool IsMatch(Row row)
        {
            var measure = row.measures[metricKey];
            return min <= measure.value && max >= measure.value;
        }
    }
}