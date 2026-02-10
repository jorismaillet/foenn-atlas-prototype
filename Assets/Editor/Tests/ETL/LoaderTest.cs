using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.ETL.Transformers;
using Assets.Scripts.Foenn.Utils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Editor.Tests.ETL
{
    public class LoaderTest
    {
        [Test]
        public void TestCreateTable()
        {
            var datasource = new WeatherHistoryDatasource();
            var loader = new SqliteLoader(datasource, "Resources/sqlite/foenn_test.db");
            var schema = new SchemaDefinition("weather_data");
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data;");
            new Transformer(datasource).TransformHeaders(schema);
            loader.connector.CreateTable(schema);
            var create = $"INSERT INTO \"weather_data\" (ID) VALUES (1);";
            loader.connector.ExecuteOperation(create);
            var res = loader.connector.ExecuteQuery(new QueryRequest("weather_data"));
            Assert.AreEqual(res.rows.Count, 1);
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data;");
        }

        [Test]
        public void TestLoad()
        {
            var datasource = new WeatherHistoryDatasource();
            var schema = new SchemaDefinition("weather_data");
            schema.AddHeaders(new List<Datafield> { new Datafield("NUM_POSTE", Datatype.INT), new Datafield("AAAAMMJJHH", Datatype.STRING) });
            var line = new List<string> { "290001", "2023010112" };
            var transformer = new Transformer(datasource);
            transformer.TransformHeaders(schema);
            transformer.TransformLine(schema, line);
            var loader = new SqliteLoader(datasource, "Resources/sqlite/foenn_test.db");
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data;");
            loader.connector.CreateTable(schema);
            loader.StartLoad(schema);
            loader.LoadLine(line);
            loader.EndLoad();
            var res = loader.connector.ExecuteQuery(new QueryRequest("weather_data"));
            Assert.AreEqual(res.rows.Count, 1);
            Assert.AreEqual(res.rows[0].geo.numPost, "290001");
            Assert.AreEqual(TimeUtils.Date("2023010112"), res.rows[0].time.start);
            Assert.AreEqual(res.rows[0].time.durationHours, 1);
            Assert.AreEqual(res.rows[0].attributes.Count, 2);
            Assert.AreEqual(res.rows[0].measures.Count, 0);
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data;");
        }
    }
}