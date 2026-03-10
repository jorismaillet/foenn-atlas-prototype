using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using System.Data;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class PrimaryKey : Field
    {
        public IndexDefinition indexDefinition;
        public bool autoIncrement;

        public PrimaryKey(string name, DbType dbType, ColumnType columnType, bool autoIncrement) : base(name, dbType, columnType)
        {
            this.autoIncrement = autoIncrement;
            this.indexDefinition = new IndexDefinition(true, this);
        }
    }
}
