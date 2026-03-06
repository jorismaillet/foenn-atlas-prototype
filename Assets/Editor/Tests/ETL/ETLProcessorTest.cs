using Assets.Scripts.Foenn;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Editor.Tests.ETL
{
    public class ETLProcessorTest
    {
        [Test]
        public void TestETLProcessor()
        {
            Env.SetDatabasePath(SqliteConnector.DATABASE_TEST_PATH);
            var datasource = new WeatherHistoryDatasource("29");
            var fileName = "Tests/Weathers/H_29_latest-2023-2024.csv";
            var processor = new ETLProcessor(
                    datasource,
                    new CSVExtractor(fileName),
                    new SqliteLoader(datasource)
                );
            processor.ProcessETL();

            var res = processor.loader.Connector().ExecuteQuery(new QueryRequest("weather_data"));
            Assert.AreEqual(res.rows.Count, 4);

            processor.loader.Connector().ExecuteOperation("DROP TABLE IF EXISTS weather_data;");
            processor.loader.Connector().ExecuteOperation("DROP TABLE IF EXISTS weather_data_staging;");
            processor.loader.Connector().ExecuteOperation("DROP TABLE IF EXISTS weather_data_metadata;");
        }
    }
}
