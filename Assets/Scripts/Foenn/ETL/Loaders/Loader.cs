using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Models;

namespace Assets.Scripts.Foenn.ETL.Loaders
{
    public abstract class Loader
    {
        protected Datasource datasource;
        public SqlConnector connector;

        protected Loader(Datasource datasource, SqlConnector connector)
        {
            this.datasource = datasource;
            this.connector = connector;
        }
        public abstract void CreateTable(Dataset dataset);
        public abstract void Load(Dataset dataset);
    }
}