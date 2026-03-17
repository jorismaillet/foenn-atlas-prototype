using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public abstract class WeatherBaseFact : Fact
    {
        public readonly Field
            timeRef,
            locationRef;

        public WeatherBaseFact(string name, Dimension time, Dimension location) : base(name, new List<Dimension>() { time, location})
        {
            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");

            Indexes.Add(new IndexDefinition(true, timeRef, locationRef));

            References.Add(timeRef);
            References.Add(locationRef);
        }
    }
}
