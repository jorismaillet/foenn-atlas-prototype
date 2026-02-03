using Assets.Scripts.Foenn.Engine.OLAP;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions {
    public interface IActivityCondition {
        public bool SuitsHour(Row row);
    }
}
