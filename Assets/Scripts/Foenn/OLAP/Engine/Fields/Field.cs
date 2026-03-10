using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;
using System.Data;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class Field : IDataField
    {
        public string name;
        public DbType dbType { get; }
        public ColumnType columnType;

        public Field(string name, DbType type, ColumnType columnType)
        {
            this.name = name;
            this.dbType = type;
            this.columnType = columnType;
        }


        public string ToSql() {
            return $"\"{name}\"";
        }
    }
}