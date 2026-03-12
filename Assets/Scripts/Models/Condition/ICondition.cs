using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.Models.Condition
{
    public interface ICondition
    {
        public bool IsMatch(Row row);
    }
}
