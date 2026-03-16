using System;

namespace Assets.Scripts.OLAP.Schema.Fields
{
    public class FieldMap
    {
        public SourceField column;

        public Field targetField;

        public Func<string, string> transform;

        private FieldMap(SourceField column, Field targetField, Func<string, string> transform = null)
        {
            this.column = column;
            this.transform = transform;
            this.targetField = targetField;
        }

        public static FieldMap Map(SourceField column, Field field)
            => new FieldMap(column, field);

        public static FieldMap Compute(SourceField column, Field field, Func<string, string> transform)
            => new FieldMap(column, field, transform);
    }
}
