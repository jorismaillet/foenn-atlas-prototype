using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class LocationDimension : Dimension
    {
        public override string Name { get; }

        public override Field PrimaryKey => Field.PK(Name);

        public override Field LookupField => PostNumber;

        public override SourceField LookupSourceAttribute { get; }

        public Field
            PostNumber,
            Latitude,
            Longitude,
            Altitude,
            PostName,
            Department;

        public override List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, LookupField),
            new IndexDefinition(false, Department)
        };

        public override List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(LookupSourceAttribute, PostNumber),
            FieldMap.Map(new SourceField("LAT", SourceFieldType.Float), Latitude),
            FieldMap.Map(new SourceField("LON", SourceFieldType.Float), Longitude),
            FieldMap.Map(new SourceField("NOM_USUEL", SourceFieldType.String), PostName),
            FieldMap.Compute(new SourceField("NUM_POSTE", SourceFieldType.String), Department, s => s.Substring(0, 2)),
            FieldMap.Map(new SourceField("ALTI", SourceFieldType.Int), Altitude)
        };

        public LocationDimension()
        {
            Name = "location_dimension";
            LookupSourceAttribute = new SourceField("NUM_POSTE", SourceFieldType.String);

            PostNumber = Field.TextAttribute(Name, "post_number");
            Latitude = Field.GeoLatAttribute(Name, "lat");
            Longitude = Field.GeoLonAttribute(Name, "lon");
            Altitude = Field.IntAttribute(Name, "altitude");
            PostName = Field.TextAttribute(Name, "post_name");
            Department = Field.TextAttribute(Name, "department");
        }
    }
}
