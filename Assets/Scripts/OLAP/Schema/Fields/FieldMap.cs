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
        public Func<int[], string[], int> selectIndex;
        public Func<string, string> transform;


        public FieldMap(SourceField sourceField, Field targetField, Func<string, string> transform = null)
        {
            this.sourceFields = new List<SourceField>() { sourceField };
            this.targetField = targetField;
            this.transform = transform;
        }
        public FieldMap(Field targetField, Func<int[], string[], int> selectIndex, params SourceField[] sourceFields)
        {
            this.sourceFields = new List<SourceField>(sourceFields);
            this.targetField = targetField;
            this.selectIndex = selectIndex;
        }

        public Func<string[], object> GetMappingResolver(string[] csvFieldNames)
        {
            if (selectIndex != null)
            {
                int[] csvIdxs = sourceFields.Select(c => CSVHelper.FindCsvIndex(c.name, csvFieldNames)).ToArray();
                var converters = sourceFields.Select(f => f.GetConverter()).ToArray();
                return line =>
                {
                    var csvIdx = selectIndex(csvIdxs, line);
                    var raw = line[csvIdx];
                    return string.IsNullOrEmpty(raw) ? DBNull.Value : converters[csvIdx](raw);
                };
            }
            else 
            {
                var field = sourceFields[0];
                var csvIdx = CSVHelper.FindCsvIndex(field.name, csvFieldNames);
                var converter = field.GetConverter();
                if (transform != null)
                {
                    return line =>
                    {
                        var raw = line[csvIdx];
                        return string.IsNullOrEmpty(raw) ? DBNull.Value : converter(transform(raw));
                    };
                }
                else
                {
                    return (line =>
                    {
                        var raw = line[csvIdx];
                        return string.IsNullOrEmpty(raw) ? DBNull.Value : converter(raw);
                    });
                }
            }
        }
    }
}
