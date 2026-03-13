using System.Data;

namespace Assets.Scripts.OLAP.Schema
{
    public class Field
    {
        public string name;

        public DbType dbType { get; }

        public AnalyticsType analyticsType;

        public string prefix;

        public bool isPrimaryKey;

        public bool autoIncrement;

        // Reference properties
        public IDimension referencedDimension;

        public string sourceCsvColumn;

        public bool IsReference => referencedDimension != null;

        private Field(string prefix, string name, DbType type, AnalyticsType analyticsType)
        {
            this.prefix = prefix;
            this.name = name;
            this.dbType = type;
            this.analyticsType = analyticsType;
        }

        // Primary key
        public static Field PK(string prefix, string name = "ID") => new Field(prefix, name, DbType.Int32, AnalyticsType.ATTRIBUTE) { isPrimaryKey = true, autoIncrement = true };

        //Attributes
        public static Field TextAttribute(string prefix, string name) => new Field(prefix, name, DbType.String, AnalyticsType.ATTRIBUTE);
        public static Field IntAttribute(string prefix, string name) => new Field(prefix, name, DbType.Int32, AnalyticsType.ATTRIBUTE);
        public static Field FloatAttribute(string prefix, string name) => new Field(prefix, name, DbType.Single, AnalyticsType.ATTRIBUTE);

        //Geo attributes
        public static Field GeoLatAttribute(string prefix, string name) => new Field(prefix, name, DbType.Single, AnalyticsType.ATTRIBUTE) { analyticsType = AnalyticsType.GEO_LAT };
        public static Field GeoLonAttribute(string prefix, string name) => new Field(prefix, name, DbType.Single, AnalyticsType.ATTRIBUTE) { analyticsType = AnalyticsType.GEO_LON };

        //Metrics
        public static Field IntMetric(string prefix, string name) => new Field(prefix, name, DbType.Int32, AnalyticsType.METRIC);
        public static Field FloatMetric(string prefix, string name) => new Field(prefix, name, DbType.Single, AnalyticsType.METRIC);

        // Foreign key
        public static Field Ref(ITable current, IDimension foreign, string foreignKey) => new Field(current.name, foreignKey, foreign.PrimaryKey.dbType, AnalyticsType.ATTRIBUTE)
        {
            referencedDimension = foreign
        };

        public string Identifier()
        {
            return $"{prefix}.{name}";
        }
    }
}
