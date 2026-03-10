using Assets.Scripts.Foenn.Engine.Sql;

namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes
{
    public class AttributeValue
    {
        public PrefixedField attribute;
        public ;

        public AttributeValue(PrefixedField attribute, string value)
        {
            this.attribute = attribute;
            this.value = value;
        }
    }
}