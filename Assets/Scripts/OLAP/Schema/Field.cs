using System.Data;

namespace Assets.Scripts.OLAP.Schema
{
    public class Field
    {
        public string name;

        public DbType dbType { get; }

        public AnalyticsType analyticsType;

        public ITable table;

        public bool isPrimaryKey;

        public bool autoIncrement;

        // Reference properties
        public IDimension referencedDimension;

        public string sourceCsvColumn;

        public bool IsReference => referencedDimension != null;

        private Field(string name, DbType type, AnalyticsType analyticsType)
        {
            this.name = name;
            this.dbType = type;
            this.analyticsType = analyticsType;
        }

        // Primary key
        public static Field PK(string name = "ID") => new Field(name, DbType.Int32, AnalyticsType.ATTRIBUTE) { isPrimaryKey = true, autoIncrement = true };

        //Attributes
        public static Field TextAttribute(string name) => new Field(name, DbType.String, AnalyticsType.ATTRIBUTE);
        public static Field IntAttribute(string name) => new Field(name, DbType.Int32, AnalyticsType.ATTRIBUTE);
        public static Field FloatAttribute(string name) => new Field(name, DbType.Single, AnalyticsType.ATTRIBUTE);

        //Geo attributes
        public static Field GeoLatAttribute(string name) => new Field(name, DbType.Single, AnalyticsType.ATTRIBUTE) { analyticsType = AnalyticsType.GEO_LAT };
        public static Field GeoLonAttribute(string name) => new Field(name, DbType.Single, AnalyticsType.ATTRIBUTE) { analyticsType = AnalyticsType.GEO_LON };

        //Metrics
        public static Field IntMetric(string name) => new Field(name, DbType.Int32, AnalyticsType.METRIC);
        public static Field FloatMetric(string name) => new Field(name, DbType.Single, AnalyticsType.METRIC);

        // Foreign key
        public static Field Ref(ITable current, IDimension foreign, string foreignKey) => new Field(foreignKey, foreign.PrimaryKey.dbType, AnalyticsType.ATTRIBUTE)
        {
            table = current,
            referencedDimension = foreign
        };

        public string Identifier()
        {
            return $"{table.name}.{name}";
        }
    }
}
