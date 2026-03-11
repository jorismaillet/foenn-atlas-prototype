namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;
    using System.Data;

    public class SourceDimension : IDimension
    {
        public static string tableName = "source";
        public static PrimaryKey ID = new PrimaryKey("ID", DbType.Int32, ColumnType.ATTRIBUTE, true);
        public static Field csvFileName = new Field("csv_file_name", DbType.String, ColumnType.ATTRIBUTE);

        public string Name => tableName;
        public PrimaryKey PrimaryKey => ID;
        public Field LookupKey => csvFileName;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, csvFileName)
        };

        public List<Field> Columns => new List<Field>() { ID, csvFileName };
    }
}
