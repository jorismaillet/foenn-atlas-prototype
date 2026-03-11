using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Result
{
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
