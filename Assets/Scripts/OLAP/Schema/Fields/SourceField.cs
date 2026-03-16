using System;
using System.Globalization;

namespace Assets.Scripts.OLAP.Schema.Fields
{
    public class SourceField
    {
        public string name;

        public SourceFieldType type;

        public SourceField(string name, SourceFieldType type)
        {
            this.name = name;
            this.type = type;
        }

        public Func<string, object> GetConverter()
        {
            return type switch
            {
                SourceFieldType.String => s => string.IsNullOrEmpty(s) ? DBNull.Value : s,
                SourceFieldType.Float => s => string.IsNullOrEmpty(s) ? DBNull.Value : double.Parse(s, CultureInfo.InvariantCulture),
                SourceFieldType.Int => s => string.IsNullOrEmpty(s) ? DBNull.Value : int.Parse(s, CultureInfo.InvariantCulture),
                _ => throw new NotImplementedException()
            };
        }
    }
}
