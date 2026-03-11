namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System.Data;

    public class Field : IDataField
    {
        public string name;
        public DbType dbType { get; }
        public ColumnType columnType;

        public ITable table;
        public AggregationKey? aggregation;

        public Field(string name, DbType type, ColumnType columnType)
        {
            this.name = name;
            this.dbType = type;
            this.columnType = columnType;
        }

        private Field(Field source, ITable table, AggregationKey? aggregation)
        {
            this.name = source.name;
            this.dbType = source.dbType;
            this.columnType = source.columnType;
            this.table = table;
            this.aggregation = aggregation;
        }

        public Field Of(ITable table) => new Field(this, table, this.aggregation);

        public Field As(AggregationKey agg) => new Field(this, this.table, agg);

        public Field Of(ITable table, AggregationKey agg) => new Field(this, table, agg);

        public string ToSql()
        {
            var col = table != null ? $"\"{table.Name}\".\"{name}\"" : $"\"{name}\"";
            return aggregation.HasValue ? $"{aggregation.Value}({col})" : col;
        }
    }

    public enum AggregationKey
    {
        SUM, MIN, MAX, AVG, D_COUNT
    }
}
