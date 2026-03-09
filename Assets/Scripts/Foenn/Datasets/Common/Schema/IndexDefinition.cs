using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class IndexDefinition
    {
        public readonly string name;
        public readonly IReadOnlyList<Datafield> fields;
        public readonly bool unique;

        public IndexDefinition(bool unique, params Datafield[] fields)
        {
            this.name = IndexName(unique, fields);
            this.unique = unique;
            this.fields = new List<Datafield>(fields);
        }

        private static string IndexName(bool unique, Datafield[] fields)
        {
            string prefix = unique ? "UIDX" : "IDX";

            var fieldNames = fields
                .Select(f => f.name.ToUpper());
            var normalizedFields = fieldNames.Select(f => Regex.Replace(f.ToLower(), "[^a-z0-9_]", "_"));
            return $"{prefix}_{string.Join("_", normalizedFields)}";
        }
    }
}
