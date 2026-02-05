using Assets.Scripts.Foenn.Engine.OLAP;

namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    public interface ICondition
    {
        public bool IsMatch(Row row);
    }
}
