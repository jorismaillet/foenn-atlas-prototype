using System;
using System.Globalization;

namespace Assets.Scripts.OLAP.Schema
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
                SourceAttributeType.String => s => string.IsNullOrEmpty(s) ? DBNull.Value : s,
                SourceAttributeType.Float => s => string.IsNullOrEmpty(s) ? DBNull.Value : double.Parse(s, CultureInfo.InvariantCulture),
                SourceAttributeType.Int => s => string.IsNullOrEmpty(s) ? DBNull.Value : int.Parse(s, CultureInfo.InvariantCulture),
                _ => throw new NotImplementedException()
            };
        }
    }
}
