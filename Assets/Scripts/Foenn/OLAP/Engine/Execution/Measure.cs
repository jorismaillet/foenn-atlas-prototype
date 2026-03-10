namespace Assets.Scripts.Foenn.Engine.OLAP.Metrics
{
    using Assets.Scripts.Foenn.ETL.Models;

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
