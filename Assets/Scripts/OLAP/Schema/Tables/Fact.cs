using System.Collections.Generic;

namespace Assets.Scripts.OLAP.Schema.Tables
{
    public abstract class Fact : Table
    {
        public readonly List<Dimension> dimensions;

        protected Fact(string name, List<Dimension> dimensions) : base(name)
        {
            this.dimensions = dimensions;
        }
    }
}
