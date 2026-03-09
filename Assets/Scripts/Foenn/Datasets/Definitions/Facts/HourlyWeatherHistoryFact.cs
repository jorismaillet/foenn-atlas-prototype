using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using Assets.Scripts.Foenn.ETL.Dimensions;
using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Datasets.Facts
{
    public class HourlyWeatherHistoryFact : IFact
    {
        public Datafield temperature = new Datafield("temperature", DbType.Double, ColumnType.METRIC);
        public Datafield rain = new Datafield("rain", DbType.Double, ColumnType.METRIC);

        public Datafield time = new Datafield("time", DbType.Int16, ColumnType.ATTRIBUTE);
        public TimeDimension timeDimension;

        public Datafield location = new Datafield("location", DbType.Int16, ColumnType.ATTRIBUTE);
        public LocationDimension locationDimension;

        public HourlyWeatherHistoryFact(TimeDimension timeDimension, LocationDimension locationDimension)
        {
            this.timeDimension = timeDimension;
            this.locationDimension = locationDimension;
        }

        public string Name => "hourly_weather_history_facts";

        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        public List<IndexDefinition> Indexes => new List<IndexDefinition>()
        {
            new IndexDefinition(true, time, location)
        };

        public List<Datafield> Columns => new List<Datafield> { temperature, rain };

        public List<Reference> References => new List<Reference>() {
            new Reference(time, timeDimension), 
            new Reference(location, locationDimension)
        };
    }
}
