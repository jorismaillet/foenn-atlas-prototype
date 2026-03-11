using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine.Result
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
