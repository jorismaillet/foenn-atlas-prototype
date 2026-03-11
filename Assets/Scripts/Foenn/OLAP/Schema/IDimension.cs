namespace Assets.Scripts.Foenn.OLAP.Schema
{
    public interface IDimension : ITable
    {
        Field LookupKey { get; }
    }
}
