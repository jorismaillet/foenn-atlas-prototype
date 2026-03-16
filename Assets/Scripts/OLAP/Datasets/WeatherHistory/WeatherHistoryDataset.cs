using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory
{
    public class WeatherHistoryDataset : Dataset
    {
        public static WeatherHistoryDataset Instance = new WeatherHistoryDataset();

        public TimeDimension time;
        public LocationDimension location;

        public WeatherCoreFact coreFact;
        public WeatherWindFact windFact;
        public WeatherTempFact tempFact;

        public override List<Dimension> Dimensions => new List<Dimension>() { time, location };
        public override List<Fact> Facts => new List<Fact>() { coreFact, windFact, tempFact };

        public WeatherHistoryDataset() : base("weather_history")
        {
            time = new TimeDimension();
            location = new LocationDimension();

            coreFact = new WeatherCoreFact(time, location);
            windFact = new WeatherWindFact(time, location);
            tempFact = new WeatherTempFact(time, location);
        }
    }
}
