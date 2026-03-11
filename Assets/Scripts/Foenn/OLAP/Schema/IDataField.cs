namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System.Data;

    public interface IDataField
    {
        public DbType dbType { get; }

        public string ToSql();
    }
}
