namespace Assets.Scripts.Foenn.Engine.SQL
{
    public sealed class SqlParam
    {
        public SqlParam(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }
}