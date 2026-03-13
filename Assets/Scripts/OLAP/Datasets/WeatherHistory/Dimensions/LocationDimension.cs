using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class LocationDimension : IDimension
    {
        public string name => "location_dimension";

        public Field PrimaryKey => Field.PK();
        public Field LookupField => PostNumber;
        public SourceAttribute LookupSourceAttribute => new SourceAttribute("NUM_POSTE", SourceAttributeType.Int);

        public static Field PostNumber = Field.IntAttribute("post_number");
        public static Field Latitude = Field.FloatAttribute("lat");
        public static Field Longitude = Field.FloatAttribute("lon");
        public static Field Altitude = Field.IntAttribute("altitude");
        public static Field PostName = Field.TextAttribute("post_name");
        public static Field Department = Field.IntAttribute("department");

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, PostNumber),
            new IndexDefinition(false, Department)
        };

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(LookupSourceAttribute, PostNumber),
            FieldMap.Map(new SourceAttribute("LAT", SourceAttributeType.Float), Latitude),
            FieldMap.Map(new SourceAttribute("LON", SourceAttributeType.Float), Longitude),
            FieldMap.Map(new SourceAttribute("NOM_USUEL", SourceAttributeType.String), PostName),
            FieldMap.Compute(new SourceAttribute("NUM_POSTE", SourceAttributeType.Int), Department, s => s.Substring(0, 2)),
            FieldMap.Map(new SourceAttribute("ALTI", SourceAttributeType.Int), Altitude)
        };
    }
}
