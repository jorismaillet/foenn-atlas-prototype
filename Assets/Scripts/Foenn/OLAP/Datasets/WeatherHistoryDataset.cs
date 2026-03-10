namespace Assets.Scripts.Foenn.ETL.Datasets
{
    using Assets.Scripts.Foenn.Atlas.Datasets.Definitions.Metadata;
    using Assets.Scripts.Foenn.Datasets;
    using Assets.Scripts.Foenn.Datasets.Facts;
    using Assets.Scripts.Foenn.Engine.Connectors;
    using Assets.Scripts.Foenn.ETL.Dimensions;
    using Mono.Data.Sqlite;
    using System.Collections.Generic;

    public class WeatherHistoryDataset
    {
        public string Name => "weather_history_dataset";

        public static TimeDimension time = new TimeDimension();

        public static LocationDimension location = new LocationDimension();

        public static SourceDimension source = new SourceDimension();

        public static WeatherFact fact = new WeatherFact(time, location);

        public static List<IDimension> Dimensions => new List<IDimension>() { time, location, source };

        public static List<IFact> Facts => new List<IFact>() { fact };

        public static List<ITable> Tables => new List<ITable>() { fact, time, location, source };

        public WeatherHistoryDataset()
        {
        }

        public static void InitTables(SqliteConnection connection)
        {
            foreach (var table in Tables)
            {
                SqliteHelper.CreateTable(connection, table);
            }
        }
    }
}
