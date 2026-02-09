using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL.Datasources
{
    public abstract class Datasource
    {
        public abstract string TableName();
        public abstract string IdColumn();
        public abstract string Identifier(Dictionary<string, int> headerIndexes, List<string> line);

        public virtual void Transform(Dataset dataset) { }
    }
}