using Assets.Scripts.Foenn.Engine.Sql;

namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    public class Measure
    {
        public PrefixedField metric;
        public float? value;

        public Measure(PrefixedField metric, float? value)
        {
            this.metric = metric;
            this.value = value;
        }
    }
}