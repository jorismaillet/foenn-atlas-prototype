using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class LocationDimension : IDimension
    {
        public string TableName => "location_dimension";

        public Field PrimaryKey => Field.PK();

        public static Field PostNumber = Field.Int("post_number");
        public static Field Latitude = Field.Double("lat");
        public static Field Longitude = Field.Double("lon");
        public static Field Altitude = Field.Double("altitude");
        public static Field PostName = Field.Text("post_name");
        public static Field Department = Field.Text("department");

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, PostNumber),
            new IndexDefinition(false, Department)
        };

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceAttribute("NUM_POSTE", SourceAttributeType.Int), PostNumber),
            FieldMap.Map(new SourceAttribute("LAT", SourceAttributeType.Float), Latitude),
            FieldMap.Map(new SourceAttribute("LON", SourceAttributeType.Float), Longitude),
            FieldMap.Map(new SourceAttribute("NOM_USUEL", SourceAttributeType.String), PostName),
            FieldMap.Compute(new SourceAttribute("NUM_POSTE", SourceAttributeType.Int), Department, s => s.Substring(0, 2)),
            FieldMap.Map(new SourceAttribute("ALTI", SourceAttributeType.Int), Altitude)
        };
    }
}
