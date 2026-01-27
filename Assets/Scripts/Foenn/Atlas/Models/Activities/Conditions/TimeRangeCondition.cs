namespace Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions {
    public class TimeRangeCondition : IActivityCondition {
        public int minHour, maxHour;

        public TimeRangeCondition(int minHour, int maxHour) {
            this.minHour = minHour;
            this.maxHour = maxHour;
        }
    }
}
