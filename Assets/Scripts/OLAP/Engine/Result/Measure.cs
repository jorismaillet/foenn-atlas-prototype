namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.OLAP.Schema;

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
