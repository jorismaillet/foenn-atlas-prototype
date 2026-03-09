using Assets.Scripts.Foenn.Atlas.Database.Datasets;
using Assets.Scripts.Foenn.Atlas.Datasets.Definitions.Metadata;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.Datasets.Facts;
using Assets.Scripts.Foenn.ETL.Dimensions;
using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.ETL.Datasets
{
    public class WeatherHistoryDataset
    {
        public string Name => "weather_data";

        public TimeDimension time = new TimeDimension();
        public LocationDimension location = new LocationDimension();
        public SourceDimension source = new SourceDimension();

        public HourlyWeatherHistoryFact fact;

        public List<IDimension> Dimensions => new List<IDimension>() { time, location, source };
        public List<IFact> Facts => new List<IFact>() { fact };

        public WeatherHistoryDataset()
        {
            fact = new HourlyWeatherHistoryFact(time, location);
        }
    }
}
