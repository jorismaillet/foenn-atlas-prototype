namespace Assets.Scripts.Foenn.Atlas.Models.Condition
{
    using Assets.Scripts.Foenn.OLAP.Query;

    public interface ICondition
    {
        public bool IsMatch(Row row);
    }
}
