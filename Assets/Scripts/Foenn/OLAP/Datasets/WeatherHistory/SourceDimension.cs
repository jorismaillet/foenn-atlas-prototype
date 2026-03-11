namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    public class SourceDimension : IDimension
    {
        public static Field csvFileName = Field.Text("csv_file_name");

        public string TableName => "source";
        public Field PrimaryKey => Field.PK();

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, csvFileName)
        };

        public List<FieldMapping> Mappings => new List<FieldMapping>();
    }
}
