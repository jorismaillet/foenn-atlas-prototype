using System.Data;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Schema.Fields
{
    public class Field
    {
        public string fieldName;

        public DbType dbType { get; }

        public AnalyticsType analyticsType;

        public string tableName;

        public bool isPrimaryKey;

        public bool autoIncrement;

        public Dimension referencedDimension;

        public string sourceCsvColumn;

        private Field(string tableName, string fieldName, DbType type, AnalyticsType analyticsType)
        {
            this.tableName = tableName;
            this.fieldName = fieldName;
            this.dbType = type;
            this.analyticsType = analyticsType;
        }

        // Primary key
        public static Field PK(string tableName, string fieldName = "id") => new Field(tableName, fieldName, DbType.Int32, AnalyticsType.ATTRIBUTE) { isPrimaryKey = true, autoIncrement = true };

        //Attributes
        public static Field TextAttribute(string tableName, string fieldName) => new Field(tableName, fieldName, DbType.String, AnalyticsType.ATTRIBUTE);

        public static Field IntAttribute(string tableName, string fieldName) => new Field(tableName, fieldName, DbType.Int32, AnalyticsType.ATTRIBUTE);

        public static Field FloatAttribute(string tableName, string fieldName) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.ATTRIBUTE);

        //Geo attributes
        public static Field GeoLatAttribute(string tableName, string fieldName) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.GEO_LAT);

        public static Field GeoLonAttribute(string tableName, string fieldName) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.GEO_LON);

        //Metrics
        public static Field IntMetric(string tableName, string fieldName) => new Field(tableName, fieldName, DbType.Int32, AnalyticsType.METRIC);

        public static Field FloatMetric(string tableName, string fieldName) => new Field(tableName, fieldName, DbType.Single, AnalyticsType.METRIC);

        // Foreign key
        public static Field Ref(ITable current, Dimension foreign, string foreignKey) => new Field(current.Name, foreignKey, foreign.PrimaryKey.dbType, AnalyticsType.ATTRIBUTE)
        {
            referencedDimension = foreign
        };

        public string Identifier()
        {
            return $"{tableName}.{fieldName}";
        }
    }
}
