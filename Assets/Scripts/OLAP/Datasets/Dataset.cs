using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;

namespace Assets.Scripts.OLAP.Datasets
{
    public abstract class Dataset
    {
        private string name;

        public abstract List<Dimension> Dimensions { get; }

        public abstract List<Fact> Facts { get; }

        public Dataset(string name)
        {
            this.name = name;
            MetadataTable = new MetadataTable(name);
        }

        public void InitTables(SqliteConnection connection)
        {
            foreach (var dim in Dimensions)
            {
                SqliteHelper.CreateTable(connection, dim);
            }
            foreach (var fact in Facts)
            {
                SqliteHelper.CreateTable(connection, fact);
            }
        }

        public MetadataTable MetadataTable;
    }
}
