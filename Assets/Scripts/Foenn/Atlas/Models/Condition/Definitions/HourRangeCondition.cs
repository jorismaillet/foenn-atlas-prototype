namespace Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions
{
    using Assets.Scripts.Foenn.Engine.Execution;

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
