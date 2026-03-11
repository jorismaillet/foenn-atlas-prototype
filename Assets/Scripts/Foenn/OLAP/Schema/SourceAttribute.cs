using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.OLAP.Schema
{
    public class SourceAttribute
    {
        public string name;
        public SourceAttributeType type;

        public SourceAttribute(string name, SourceAttributeType type)
        {
            this.name = name;
            this.type = type;
        }

        public Func<string, object> GetConverter()
        {
            return type switch
            {
                SourceAttributeType.String => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)s,
                SourceAttributeType.Float => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)double.Parse(s, CultureInfo.InvariantCulture),
                SourceAttributeType.Int => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)int.Parse(s, CultureInfo.InvariantCulture),
                _ => s => string.IsNullOrEmpty(s) ? DBNull.Value : (object)s
            };
        }
    }
}
