namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes
{
    using Assets.Scripts.Foenn.OLAP.Engine.Sql;

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
