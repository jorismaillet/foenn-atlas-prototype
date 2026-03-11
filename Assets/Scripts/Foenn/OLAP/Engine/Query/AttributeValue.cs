namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.OLAP.Schema;

    public class AttributeValue
    {
        public IDataField attribute;

        public string value;

        public AttributeValue(IDataField attribute, string value)
        {
            this.attribute = attribute;
            this.value = value;
        }
    }
}
