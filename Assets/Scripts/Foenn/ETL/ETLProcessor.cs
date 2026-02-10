using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Transformers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Assets.Scripts.Foenn.ETL.CSV
{
    public class ETLProcessor
    {
        public Datasource datasource;
        public Extractor extractor;
        public Transformer transformer;
        public Loader loader;

        public ETLProcessor(Datasource datasource, Extractor extractor, Transformer transformer, Loader loader)
        {
            this.datasource = datasource;
            this.extractor = extractor;
            this.transformer = transformer;
            this.loader = loader;
        }

        public void ProcessETL()
        {
            var dataset = new Dataset();
            extractor.Extract(dataset);
            transformer.Transform(dataset);
            loader.connector.OpenSession();
            loader.CreateTable(dataset);
            loader.Load(dataset);
            loader.connector.CloseSession();
        }
    }
}