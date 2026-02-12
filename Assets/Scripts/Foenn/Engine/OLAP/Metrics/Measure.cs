namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    public class Measure
    {
        public Metric metric;
        public float? value;

        public Measure(Metric metric, float? value)
        {
            this.metric = metric;
            this.value = value;
        }
    }
}