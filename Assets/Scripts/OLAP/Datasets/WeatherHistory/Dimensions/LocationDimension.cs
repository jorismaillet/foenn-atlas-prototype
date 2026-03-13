using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class LocationDimension : IDimension
    {
        public static string Name => "location_dimension";
        public string name => Name;

        public Field PrimaryKey => Field.PK(Name);
        public Field LookupField => PostNumber;
        public SourceAttribute LookupSourceAttribute => new SourceAttribute("NUM_POSTE", SourceAttributeType.String);

        public static Field PostNumber = Field.TextAttribute(Name, "post_number");
        public static Field Latitude = Field.GeoLatAttribute(Name, "lat");
        public static Field Longitude = Field.GeoLonAttribute(Name, "lon");
        public static Field Altitude = Field.IntAttribute(Name, "altitude");
        public static Field PostName = Field.TextAttribute(Name, "post_name");
        public static Field Department = Field.TextAttribute(Name, "department");

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, LookupField),
            new IndexDefinition(false, Department)
        };

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(LookupSourceAttribute, PostNumber),
            FieldMap.Map(new SourceAttribute("LAT", SourceAttributeType.Float), Latitude),
            FieldMap.Map(new SourceAttribute("LON", SourceAttributeType.Float), Longitude),
            FieldMap.Map(new SourceAttribute("NOM_USUEL", SourceAttributeType.String), PostName),
            FieldMap.Compute(new SourceAttribute("NUM_POSTE", SourceAttributeType.String), Department, s => s.Substring(0, 2)),
            FieldMap.Map(new SourceAttribute("ALTI", SourceAttributeType.Int), Altitude)
        };
    }
}
