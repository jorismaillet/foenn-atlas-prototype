using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory
{
    public class WeatherHistoryDataset
    {
        public static TimeDimension time = new TimeDimension();

        public static LocationDimension location = new LocationDimension();

        public static WeatherFact fact = new WeatherFact(time, location);

        public static WeatherExtraFact extraFact = new WeatherExtraFact(time, location);

        public static List<IDimension> Dimensions => new List<IDimension>() { time, location };

        public static List<IFact> Facts => new List<IFact>() { fact, extraFact };

        public static List<ITable> Tables => new List<ITable>() { fact, extraFact, time, location };

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
