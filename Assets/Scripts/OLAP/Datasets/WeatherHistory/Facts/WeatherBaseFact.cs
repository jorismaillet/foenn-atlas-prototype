using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public abstract class WeatherBaseFact : Fact
    {
        public readonly Field
            timeRef,
            locationRef;

        protected TimeDimension time;
        protected LocationDimension location;

        public WeatherBaseFact(string name, TimeDimension time, LocationDimension location) : base(name, new List<Dimension>() { time, location })
        {
            this.time = time;
            this.location = location;

            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");

            Indexes.Add(new IndexDefinition(true, timeRef, locationRef));
            Indexes.Add(new IndexDefinition(false, locationRef));

            References.Add(timeRef);
            References.Add(locationRef);
        }
    }
}
