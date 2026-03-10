namespace Assets.Scripts.Foenn.OLAP.Engine.Sql
{
    using System.Data;

    public interface IDataField
    {
        public DbType dbType { get; }

        public string ToSql();
    }
}
