using Assets.Scripts.Foenn.OLAP.Query;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions
{
    public class HourRangeCondition : ICondition
    {
        public int minHour, maxHour;

        public HourRangeCondition(int minHour, int maxHour)
        {
            this.minHour = minHour;
            this.maxHour = maxHour;
        }

        public bool IsMatch(Row row)
        {
            return row.time.start.Hour >= minHour && row.time.End().Hour <= maxHour;
        }
    }
}
