using System.Data;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Schema.Fields
{
    public class Field
    {
        public string fieldName;

        public string displayName;

        public DbType dbType { get; }

        public AnalyticsType analyticsType;

        public string tableName;

        public bool isPrimaryKey;

        public bool autoIncrement;

        public Dimension referencedDimension;

        public string sourceCsvColumn;

        public string format;

        private Field(string tableName, string fieldName, DbType type, AnalyticsType analyticsType, string displayName = null, string format = null)
        {
            this.tableName = tableName;
            this.fieldName = fieldName;
            this.displayName = displayName;
            this.dbType = type;
            this.analyticsType = analyticsType;
            this.format = format;
        }

        // Primary key
        public static Field PK(string tableName, string fieldName = "id") => new Field(tableName, fieldName, DbType.Int32, AnalyticsType.ATTRIBUTE) { isPrimaryKey = true, autoIncrement = true };

        //Attributes
        public static Field TextAttribute(string tableName, string fieldName, string displayName) => new Field(tableName, fieldName, DbType.String, AnalyticsType.ATTRIBUTE, displayName);

        public static Field IntAttribute(string tableName, string fieldName, string displayName) => new Field(tableName, fieldName, DbType.Int32, AnalyticsType.ATTRIBUTE, displayName);

        public static Field FloatAttribute(string tableName, string fieldName, string displayName) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.ATTRIBUTE, displayName);

        //Geo attributes
        public static Field GeoLatAttribute(string tableName, string fieldName, string displayName) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.GEO_LAT, displayName);

        public static Field GeoLonAttribute(string tableName, string fieldName, string displayName) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.GEO_LON, displayName);

        //Metrics
        public static Field IntMetric(string tableName, string fieldName, string displayName, string format = null) => new Field(tableName, fieldName, DbType.Int32, AnalyticsType.METRIC, displayName, format);

        public static Field FloatMetric(string tableName, string fieldName, string displayName, string format = null) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.METRIC, displayName, format);

        // Foreign key
        public static Field Ref(Table current, Dimension foreign, string foreignKey) => new Field(current.Name, foreignKey, foreign.PrimaryKey.dbType, AnalyticsType.ATTRIBUTE)
        {
            referencedDimension = foreign
        };

        public string Identifier()
        {
            return $"{tableName}.{fieldName}";
        }

        public string Value(Row row)
        {
            return $"{row.values[this]} {format}";
        }
    }
}
