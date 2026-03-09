using Assets.Scripts.Foenn.Atlas.Datasets.Definitions.Metadata;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.Datasets.Facts;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Dimensions;
using Mono.Data.Sqlite;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL.Datasets
{
    public class WeatherHistoryDataset
    {
        public string Name => "weather_history_dataset";

        public TimeDimension time = new TimeDimension();
        public LocationDimension location = new LocationDimension();
        public SourceDimension source = new SourceDimension();

        public HourlyWeatherHistoryFact fact;

        public List<IDimension> Dimensions => new List<IDimension>() { time, location, source };
        public List<IFact> Facts => new List<IFact>() { fact };
        public List<ITable> Tables => new List<ITable>() { fact, time, location, source };

        public WeatherHistoryDataset()
        {
            fact = new HourlyWeatherHistoryFact(time, location);
        }

        public void InitTables(SqliteConnection connection) {
            foreach (var table in Tables) {
                SqliteHelper.CreateTable(connection, table);
            }
        }
    }
}
