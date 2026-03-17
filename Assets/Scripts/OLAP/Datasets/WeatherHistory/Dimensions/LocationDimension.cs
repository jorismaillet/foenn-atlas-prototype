using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class LocationDimension : Dimension
    {
        public override Field LookupField => PostNumber;

        public readonly Field
            PostNumber,
            Latitude,
            Longitude,
            Altitude,
            PostName,
            Department;

        public LocationDimension() : base(
            "location_dimension",
            new SourceField("NUM_POSTE", SourceFieldType.String)
        )
        {
            PostNumber = Field.TextAttribute(Name, "post_number", "Post number");
            Latitude = Field.GeoLatAttribute(Name, "lat", "Latitude");
            Longitude = Field.GeoLonAttribute(Name, "lon", "Longitude");
            Altitude = Field.IntAttribute(Name, "altitude", "Altitude");
            PostName = Field.TextAttribute(Name, "post_name", "Post name");
            Department = Field.TextAttribute(Name, "department", "Department");

            Indexes.Add(new IndexDefinition(true, LookupField));
            Indexes.Add(new IndexDefinition(false, Department));

            Mappings.Add(FieldMap.Map(LookupSourceAttribute, PostNumber));
            Mappings.Add(FieldMap.Map(new SourceField("LAT", SourceFieldType.Float), Latitude));
            Mappings.Add(FieldMap.Map(new SourceField("LON", SourceFieldType.Float), Longitude));
            Mappings.Add(FieldMap.Map(new SourceField("NOM_USUEL", SourceFieldType.String), PostName));
            Mappings.Add(FieldMap.Compute(new SourceField("NUM_POSTE", SourceFieldType.String), Department, s => s.Substring(0, 2)));
            Mappings.Add(FieldMap.Map(new SourceField("ALTI", SourceFieldType.Int), Altitude));
        }
    }
}
