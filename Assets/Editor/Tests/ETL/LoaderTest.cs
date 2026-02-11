using Assets.Scripts.Foenn;
using Assets.Scripts.Foenn.Engine.Connectors;
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
            Env.SetDatabasePath(SqliteConnector.DATABASE_TEST_PATH);
            var datasource = new WeatherHistoryDatasource();
            var loader = new SqliteLoader(datasource);
            var schema = new SchemaDefinition("weather_data");
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data;");
            new Transformer(datasource).TransformColumns(schema);
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
            Env.SetDatabasePath(SqliteConnector.DATABASE_TEST_PATH);
            var datasource = new WeatherHistoryDatasource();
            var schema = new SchemaDefinition("weather_data");
            schema.AddColumns(new List<Datafield> { new Datafield("NUM_POSTE", Datatype.INT), new Datafield("AAAAMMJJHH", Datatype.STRING) });
            var line = new string[] { "290001", "2023010112" };
            var transformer = new Transformer(datasource);
            datasource.PrepareTranformer(schema);
            transformer.TransformColumns(schema);
            var extraColumns = datasource.GetExtraColumns(line);
            transformer.TransformLine(schema, line);
            var loader = new SqliteLoader(datasource);
            loader.connector.DropStagingTable(schema);
            loader.connector.CreateStagingTable(schema);
            loader.StartStaging(schema);
            loader.StageLine(line, extraColumns);
            loader.CommitStaging();
            var res = loader.connector.ExecuteQuery(new QueryRequest("weather_data_staging"));
            Assert.AreEqual(res.rows.Count, 1);
            Assert.AreEqual(res.rows[0].geo.numPost, "290001");
            Assert.AreEqual(TimeUtils.Date("2023010112"), res.rows[0].time.start);
            Assert.AreEqual(res.rows[0].time.durationHours, 1);
            Assert.AreEqual(res.rows[0].attributes.Count, 1);
            Assert.AreEqual(res.rows[0].attributes[0].key, WeatherHistoryAttributeKey.ID);
            Assert.AreEqual(res.rows[0].measures.Count, 0);
            loader.connector.DropStagingTable(schema);
        }
    }
}