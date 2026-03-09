using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using System.Data;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class Datafield
    {
        public string name;
        public DbType dbType;
        public ColumnType columnType;

        public Datafield(string name, DbType type, ColumnType columnType)
        {
            this.name = name;
            this.dbType = type;
            this.columnType = columnType;
        }
    }
}