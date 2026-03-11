namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    public class LocationDimension : IDimension
    {
        public static Field PostNumber = Field.Int("post_number");
        public static Field Latitude = Field.Double("lat");
        public static Field Longitude = Field.Double("lon");
        public static Field PostName = Field.Text("post_name");
        public static Field Department = Field.Text("department");

        public string TableName => "location";
        public Field PrimaryKey => Field.PK();

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, PostNumber),
            new IndexDefinition(false, Department)
        };

        public List<FieldMapping> Mappings => new List<FieldMapping>()
        {
            FieldMapping.MapInt("NUM_POSTE", "post_number"),
            FieldMapping.Map("LAT", Latitude),
            FieldMapping.Map("LON", Longitude),
            FieldMapping.MapText("NOM_USUEL", "post_name"),
            FieldMapping.Compute("NUM_POSTE", Department, s => s.Substring(0, 2)),
            FieldMapping.MapInt("ALTI", "altitude")
        };
    }
}
