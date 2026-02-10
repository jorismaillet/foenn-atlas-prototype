using Assets.Scripts.Foenn.Engine.Execution;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public class NamedCondition : ICondition
    {
        public string name;
        public ICondition condition;

        public NamedCondition(string name, ICondition condition)
        {
            this.name = name;
            this.condition = condition;
        }

        public bool IsMatch(Row row)
        {
            return condition.IsMatch(row);
        }
    }
}