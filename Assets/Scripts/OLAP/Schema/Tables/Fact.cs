using System.Collections.Generic;
using Assets.Scripts.ETL.Loaders;
using Mono.Data.Sqlite;

namespace Assets.Scripts.OLAP.Schema.Tables
{
    public abstract class Fact : Table
    {
        public readonly List<Dimension> dimensions;

        protected Fact(string name, List<Dimension> dimensions) : base(name)
        {
            this.dimensions = dimensions;
        }

        public virtual void BuildDerivedFact(SqliteConnection connection, Dictionary<Dimension, DimensionCache> caches) { }
    }
}
