namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    using Assets.Scripts.Foenn.Engine.Execution;

    public interface ICondition
    {
        public bool IsMatch(Row row);
    }
}
