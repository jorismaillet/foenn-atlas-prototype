namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;
    using System.Data;

    public class LocationDimension : IDimension
    {
        public static PrimaryKey ID = new PrimaryKey("ID", DbType.Int32, ColumnType.ATTRIBUTE, true);
        public static Field PostNumber = new Field("post_number", DbType.Int32, ColumnType.ATTRIBUTE);
        public static Field Latitude = new Field("lat", DbType.Double, ColumnType.ATTRIBUTE);
        public static Field Longitude = new Field("lon", DbType.Double, ColumnType.ATTRIBUTE);
        public static Field PostName = new Field("post_name", DbType.String, ColumnType.ATTRIBUTE);
        public static Field Department = new Field("department", DbType.String, ColumnType.ATTRIBUTE);
        public static Field Altitude = new Field("altitude", DbType.Int16, ColumnType.ATTRIBUTE);

        public string Name => "location";
        public PrimaryKey PrimaryKey => ID;
        public Field LookupKey => PostNumber;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, PostNumber),
            new IndexDefinition(false, Department)
        };

        public List<Field> Columns => new List<Field>() { ID, PostNumber, Latitude, Longitude, PostName, Department, Altitude };
    }
}
