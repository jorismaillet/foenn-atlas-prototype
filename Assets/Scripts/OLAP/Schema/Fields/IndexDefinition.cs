using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Scripts.OLAP.Schema.Fields
{
    public class IndexDefinition
    {
        public readonly string name;

        public readonly IReadOnlyList<Field> fields;

        public readonly bool unique;

        public IndexDefinition(bool unique, params Field[] fields)
        {
            this.name = IndexName(unique, fields);
            this.unique = unique;
            this.fields = new List<Field>(fields);
        }

        public static string IndexName(bool unique, Field[] fields)
        {
            string prefix = unique ? "UIDX" : "IDX";

            var fieldNames = fields
                .Select(f => f.fieldName.ToUpper());
            var normalizedFields = fieldNames.Select(f => Regex.Replace(f, "[^a-z0-9_]", "_"));
            return $"{prefix}_{string.Join("_", normalizedFields)}";
        }
    }
}
