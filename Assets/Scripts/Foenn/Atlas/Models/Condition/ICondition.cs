using Assets.Scripts.Foenn.Engine.Execution;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public interface ICondition
    {
        public bool IsMatch(Row row);
    }
}
