namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.OLAP.Schema;

    public class AttributeValue
    {
        public Field attribute;

        public string value;

        public AttributeValue(Field attribute, string value)
        {
            this.attribute = attribute;
            this.value = value;
        }
    }
}
