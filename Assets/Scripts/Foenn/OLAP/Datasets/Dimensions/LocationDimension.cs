namespace Assets.Scripts.Foenn.ETL.Dimensions
{
    using Assets.Scripts.Foenn.Atlas.Datasets.Common;
    using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
    using Assets.Scripts.Foenn.ETL.Models;
    using System.Collections.Generic;
    using System.Data;

    public class LocationDimension : IDimension
    {
        public static Field Latitude = new Field("lat", DbType.Double, ColumnType.ATTRIBUTE);

        public static Field Longitude = new Field("lon", DbType.Double, ColumnType.ATTRIBUTE);

        public static Field PostName = new Field("post_name", DbType.String, ColumnType.ATTRIBUTE);

        public static Field PostNumber = new Field("post_number", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field Department = new Field("department", DbType.String, ColumnType.ATTRIBUTE);

        public static Field Altitude = new Field("altitude", DbType.Int16, ColumnType.ATTRIBUTE);

        public string Name => "location";

        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(false, Department),
            new IndexDefinition(false, PostNumber),
            new IndexDefinition(false, PostName),
            new IndexDefinition(false, Latitude, Longitude)
        };

        public List<Field> Columns => new List<Field>() { Latitude, Longitude, PostName, PostNumber, Department, Altitude };

        public List<Reference> References => new List<Reference>();
    }
}
