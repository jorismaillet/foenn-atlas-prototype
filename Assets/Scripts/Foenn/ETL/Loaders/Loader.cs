using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.ETL.Datasources;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;

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