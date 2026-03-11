using System.Data;

namespace Assets.Scripts.OLAP.Schema
{
    public class Field
    {
        public string name;

        public DbType dbType { get; }

        public AnalyticsType analyticsType;

        public ITable table;

        public AggregationKey? aggregation;

        public bool isPrimaryKey;

        public bool autoIncrement;

        // Reference properties
        public IDimension referencedDimension;

        public string sourceCsvColumn;

        public bool IsReference => referencedDimension != null;

        public Field(string name, DbType type, AnalyticsType analyticsType)
        {
            this.name = name;
            this.dbType = type;
            this.analyticsType = analyticsType;
        }

        private Field(Field source, ITable table, AggregationKey? aggregation)
        {
            this.name = source.name;
            this.dbType = source.dbType;
            this.analyticsType = source.analyticsType;
            this.isPrimaryKey = source.isPrimaryKey;
            this.autoIncrement = source.autoIncrement;
            this.referencedDimension = source.referencedDimension;
            this.sourceCsvColumn = source.sourceCsvColumn;
            this.table = table;
            this.aggregation = aggregation;
        }

        public static Field Metric(string name) => new Field(name, DbType.Double, AnalyticsType.METRIC);

        public static Field Text(string name) => new Field(name, DbType.String, AnalyticsType.ATTRIBUTE);

        public static Field Int(string name) => new Field(name, DbType.Int32, AnalyticsType.ATTRIBUTE);

        public static Field Int16(string name) => new Field(name, DbType.Int16, AnalyticsType.ATTRIBUTE);

        public static Field Int64(string name) => new Field(name, DbType.Int64, AnalyticsType.ATTRIBUTE);

        public static Field Double(string name) => new Field(name, DbType.Double, AnalyticsType.ATTRIBUTE);

        public static Field PK(string name = "ID") => new Field(name, DbType.Int32, AnalyticsType.ATTRIBUTE) { isPrimaryKey = true, autoIncrement = true };

        public static Field Ref(IDimension dimension, string fieldName) => new Field(fieldName, dimension.PrimaryKey.dbType, AnalyticsType.ATTRIBUTE)
        {
            referencedDimension = dimension
        };

        public Field Of(ITable table) => new Field(this, table, this.aggregation);

        public Field As(AggregationKey agg) => new Field(this, this.table, agg);

        public Field Of(ITable table, AggregationKey agg) => new Field(this, table, agg);

        public string ToSql()
        {
            var col = table != null ? $"\"{table.TableName}\".\"{name}\"" : $"\"{name}\"";
            return aggregation.HasValue ? $"{aggregation.Value}({col})" : col;
        }
    }

    public enum AggregationKey
    {
        SUM, MIN, MAX, AVG, D_COUNT
    }
}
