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
        public abstract void StartStaging(SchemaDefinition schema);
        public abstract void StageLine(string[] line, string[] extraLines);
        public abstract void CommitStaging();
        public abstract void MergeStaging(SchemaDefinition schema);
    }
}