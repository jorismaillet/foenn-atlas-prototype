using System;

namespace Assets.Scripts.OLAP.Schema
{
    public class FieldMap
    {
        public SourceAttribute column;

        public Field targetField;

        public Func<string, string> transform;

        private FieldMap(SourceAttribute column, Field targetField, Func<string, string> transform = null)
        {
            this.column = column;
            this.transform = transform;
            this.targetField = targetField;
        }

        public static FieldMap Map(SourceAttribute column, Field field)
            => new FieldMap(column, field);

        public static FieldMap Compute(SourceAttribute column, Field field, Func<string, string> transform)
            => new FieldMap(column, field, transform);
    }
}
