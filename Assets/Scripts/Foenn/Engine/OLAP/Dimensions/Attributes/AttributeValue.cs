using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes
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