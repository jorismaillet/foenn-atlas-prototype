using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class LocationDimension : Dimension
    {
        public override Field LookupField => PostNumber;

        public readonly Field
            PostNumber,
            PostName,
            Department,
            Latitude,
            Longitude,
            Altitude;

        public LocationDimension() : base(
            "location_dimension",
            new SourceField("NUM_POSTE", SourceFieldType.String)
        )
        {
            PostNumber = Field.TextAttribute(Name, "post_number", "Post number");
            PostName = Field.TextAttribute(Name, "post_name", "Post name");
            Department = Field.TextAttribute(Name, "department", "Department");
            Latitude = Field.GeoLatAttribute(Name, "lat", "Latitude");
            Longitude = Field.GeoLonAttribute(Name, "lon", "Longitude");
            Altitude = Field.IntAttribute(Name, "altitude", "Altitude");

            Indexes.Add(new IndexDefinition(true, LookupField));
            Indexes.Add(new IndexDefinition(false, PostName));
            Indexes.Add(new IndexDefinition(false, Department));

            Mappings.Add(new FieldMap(LookupSourceAttribute, PostNumber));
            Mappings.Add(new FieldMap(new SourceField("NOM_USUEL", SourceFieldType.String), PostName));
            Mappings.Add(new FieldMap(new SourceField("NUM_POSTE", SourceFieldType.String), Department, s => s.Substring(0, 2)));
            Mappings.Add(new FieldMap(new SourceField("LAT", SourceFieldType.Float), Latitude));
            Mappings.Add(new FieldMap(new SourceField("LON", SourceFieldType.Float), Longitude));
            Mappings.Add(new FieldMap(new SourceField("ALTI", SourceFieldType.Int), Altitude));
        }
    }
}
