namespace Assets.Scripts.Foenn.Engine.Attributes
{
    public class AttributeValue
    {
        public Attribute attribute;
        public string value;

        public AttributeValue(Attribute attribute, string value)
        {
            this.attribute = attribute;
            this.value = value;
        }
    }
}