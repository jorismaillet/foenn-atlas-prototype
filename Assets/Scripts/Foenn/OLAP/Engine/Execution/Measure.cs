using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Models;

namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    public class Measure
    {
        public Field metric;
        public float? value;

        public Measure(Field metric, float? value)
        {
            this.metric = metric;
            this.value = value;
        }
    }
}