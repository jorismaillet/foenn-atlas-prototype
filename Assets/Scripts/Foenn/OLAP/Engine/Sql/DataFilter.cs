namespace Assets.Scripts.Foenn.OLAP.Sql
{
    using Assets.Scripts.Foenn.OLAP.Fields;
    using System.Collections.Generic;
    using System.Linq;

    public class DataFilter : Filter
    {
        public List<object> selectedValues;

        public DataFilterMode mode;

        public DataFilter(PrefixedField column, DataFilterMode mode, params object[] selectedValues) : base(column)
        {
            this.selectedValues = selectedValues.ToList();
            this.mode = mode;
        }

        public override string ToSql()
        {
            string filterOperator = null;
            string filteredAttributes = null;
            if (selectedValues.Count > 1)
            {
                filterOperator = mode.Equals(DataFilterMode.INCLUDE) ? " IN" : " NOT IN";
                filteredAttributes = $"({string.Join(", ", selectedValues.Select(v => ValueToSql(v)))})";
            }
            else
            {
                filterOperator = mode.Equals(DataFilterMode.INCLUDE) ? "=" : "!=" + " ";
                filteredAttributes = ValueToSql(selectedValues[0]);
            }
            return $"{filteredField.ToSql()}{filterOperator}{filteredAttributes}";
        }

        private string ValueToSql(object value)
        {
            if (value is string)
            {
                return $"\"{value}\"";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
