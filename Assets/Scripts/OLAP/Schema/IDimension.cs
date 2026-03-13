namespace Assets.Scripts.OLAP.Schema
{
    public interface IDimension : ITable
    {
        public Field LookupField { get; }
        public SourceAttribute LookupSourceAttribute { get; }
    }
}
