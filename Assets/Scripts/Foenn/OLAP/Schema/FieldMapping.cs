namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System;

    public class FieldMapping
    {
        public string csvColumn;
        public Field targetField;
        public Func<string, object> transform;

        private FieldMapping(string csvColumn, Field targetField, Func<string, object> transform = null)
        {
            this.csvColumn = csvColumn;
            this.targetField = targetField;
            this.transform = transform;
        }

        public static FieldMapping Map(string csvColumn, Field field) 
            => new FieldMapping(csvColumn, field);

        public static FieldMapping Map(string csvColumn, string fieldName) 
            => new FieldMapping(csvColumn, Field.Metric(fieldName));

        public static FieldMapping MapText(string csvColumn, string fieldName) 
            => new FieldMapping(csvColumn, Field.Text(fieldName));

        public static FieldMapping MapInt(string csvColumn, string fieldName) 
            => new FieldMapping(csvColumn, Field.Int(fieldName));

        public static FieldMapping Compute(string csvColumn, Field field, Func<string, object> transform) 
            => new FieldMapping(csvColumn, field, transform);

        public static FieldMapping Compute(string csvColumn, string fieldName, Func<string, object> transform) 
            => new FieldMapping(csvColumn, Field.Metric(fieldName), transform);
    }
}
