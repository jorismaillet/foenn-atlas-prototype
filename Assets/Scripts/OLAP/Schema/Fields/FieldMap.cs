using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.OLAP.Schema.Fields
{
    public class FieldMap
    {
        public List<SourceField> sourceFields;
        public Field targetField;
        public Func<string, string> transform;

        public FieldMap(SourceField sourceField, Field targetField, Func<string, string> transform = null)
        {
            this.targetField = targetField;
            this.sourceFields = new List<SourceField>() { sourceField };
            this.transform = transform;
        }
        public FieldMap(Field targetField, params SourceField[] sourceFields)
        {
            this.targetField = targetField;
            this.sourceFields = new List<SourceField>(sourceFields);
        }

        public Func<string[], object> GetMappingResolver(string[] csvFieldNames)
        {
            int count = sourceFields.Count;
            var indices = new int[count];
            var converters = new Func<string, object>[count];
            for (int i = 0; i < count; i++)
            {
                indices[i] = CSVHelper.FindCsvIndex(sourceFields[i].name, csvFieldNames);
                converters[i] = sourceFields[i].GetConverter();
            }

            if (count == 1)
            {
                var idx = indices[0];
                var converter = converters[0];

                if (transform != null)
                {
                    var t = transform;
                    return line =>
                    {
                        var raw = line[idx];
                        return !string.IsNullOrEmpty(raw) ? converter(t(raw)) : DBNull.Value;
                    };
                }

                return line =>
                {
                    var raw = line[idx];
                    return !string.IsNullOrEmpty(raw) ? converter(raw) : DBNull.Value;
                };
            }

            if (transform != null)
            {
                var t = transform;
                return line =>
                {
                    for (int i = 0; i < indices.Length; i++)
                    {
                        var raw = line[indices[i]];
                        if (!string.IsNullOrEmpty(raw))
                            return converters[i](t(raw));
                    }
                    return DBNull.Value;
                };
            }

            return line =>
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    var raw = line[indices[i]];
                    if (!string.IsNullOrEmpty(raw))
                        return converters[i](raw);
                }
                return DBNull.Value;
            };
        }
    }
}
