using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory
{
    public class WeatherHistoryDataset
    {
        public static TimeDimension time = new TimeDimension();

        public static LocationDimension location = new LocationDimension();

        public static WeatherCoreFact coreFact = new WeatherCoreFact(time, location);
        public static WeatherWindFact windFact = new WeatherWindFact(time, location);
        public static WeatherTempFact tempFact = new WeatherTempFact(time, location);

        public static List<IDimension> Dimensions => new List<IDimension>() { time, location };

        public static List<IFact> Facts => new List<IFact>() { coreFact, windFact, tempFact };

        public static List<ITable> Tables => new List<ITable>() { coreFact, windFact, tempFact, time, location };

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
