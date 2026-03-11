using Assets.Scripts.OLAP.Engine.Result;

namespace Assets.Scripts.Models.Condition
{
    public interface ICondition
    {
        public bool IsMatch(Row row);
    }
}
