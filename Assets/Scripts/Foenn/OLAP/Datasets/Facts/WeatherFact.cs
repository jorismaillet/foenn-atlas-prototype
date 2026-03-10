using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using Assets.Scripts.Foenn.ETL.Dimensions;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;
using System.Data;

namespace Assets.Scripts.Foenn.Datasets.Facts
{
    public class WeatherFact : IFact
    {
        public string Name => "hourly_weather_history_facts";
        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        public static Field temperature = new Field("temperature", DbType.Double, ColumnType.METRIC);
        public static Field temperature_10 = new Field("temperature_10", DbType.Double, ColumnType.METRIC);
        public static Field temperature_20 = new Field("temperature_20", DbType.Double, ColumnType.METRIC);
        public static Field temperature_100 = new Field("temperature_100", DbType.Double, ColumnType.METRIC);
        public static Field rain = new Field("rain", DbType.Double, ColumnType.METRIC);
        public static Field rain_1 = new Field("rain_1", DbType.Double, ColumnType.METRIC);
        public static Field wind = new Field("wind", DbType.Double, ColumnType.METRIC);
        public static Field gust = new Field("gust", DbType.Double, ColumnType.METRIC);
        public static Field timeId = new Field("time_id", DbType.Int16, ColumnType.ATTRIBUTE);
        public static Field locationId = new Field("location_id", DbType.Int16, ColumnType.ATTRIBUTE);

        public Reference timeReference, locationReference;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>()
        {
            new IndexDefinition(true, timeId, locationId)
        };

        public List<Field> Columns => new List<Field> { PrimaryKey, temperature, rain };
        public WeatherFact(TimeDimension time, LocationDimension location) {
            timeReference = new Reference(time, timeId);
            locationReference = new Reference(time, locationId);
        }
    }
}
