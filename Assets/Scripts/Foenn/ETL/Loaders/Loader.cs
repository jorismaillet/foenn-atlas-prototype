using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL.Loaders
{
    public abstract class Loader
    {
        protected Datasource datasource;

        protected Loader(Datasource datasource)
        {
            this.datasource = datasource;
        }
        public abstract SqlConnector Connector();
        public abstract void StartLoad(SchemaDefinition schema);
        public abstract void LoadLine(List<string> line);
        public abstract void EndLoad();
    }
}