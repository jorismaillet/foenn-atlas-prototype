using Assets.Scripts.Foenn.Engine.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions
{
    public abstract class Dimension
    {
        public List<AttributeValue> attributeValues;

        protected Dimension(params AttributeValue[] attributeValues)
        {
            this.attributeValues = attributeValues.ToList();
        }
    }
}