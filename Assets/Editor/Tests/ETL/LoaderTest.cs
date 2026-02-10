using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.CSV;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Transformers;
using Assets.Scripts.Foenn.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Assets.Editor.Tests.ETL
{
    public class LoaderTest
    {
        [Test]
        public void TestCreateTable()
        {
            var datasource = new WeatherHistoryDatasource();
            var loader = new SqliteLoader(datasource, "Resources/sqlite/foenn_test.db");
            var dataset = new Dataset();
            loader.connector.OpenSession();
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data");
            new Transformer(datasource).AddIdColumn(dataset);
            loader.CreateTable(dataset);
            var create = $"INSERT INTO \"weather_data\" (ID) VALUES (1)";
            loader.connector.ExecuteOperation(create);
            var res = loader.connector.ExecuteQuery(new QueryRequest("weather_data"));
            Assert.AreEqual(res.rows.Count, 1);
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data");
            loader.connector.CloseSession();
        }

        [Test]
        public void TestLoad()
        {
            var datasource = new WeatherHistoryDatasource();
            var dataset = new Dataset();
            dataset.fields.AddRange(new List<Datafield> { new Datafield("NUM_POSTE", Datatype.INT), new Datafield("AAAAMMJJHH", Datatype.STRING) });
            dataset.lines.AddRange(new List<List<string>> { new List<string> { "290001", "2023010112" } });
            new Transformer(datasource).Transform(dataset);
            var loader = new SqliteLoader(datasource, "Resources/sqlite/foenn_test.db");
            loader.connector.OpenSession();
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data");
            loader.CreateTable(dataset);
            loader.Load(dataset);
            var res = loader.connector.ExecuteQuery(new QueryRequest("weather_data"));
            Assert.AreEqual(res.rows.Count, 1);
            Assert.AreEqual(res.rows[0].geo.numPost, "290001");
            Assert.AreEqual(TimeUtils.Date("2023010112"), res.rows[0].time.start);
            Assert.AreEqual(res.rows[0].time.durationHours, 1);
            Assert.AreEqual(res.rows[0].attributes.Count, 2);
            Assert.AreEqual(res.rows[0].measures.Count, 0);
            loader.connector.ExecuteOperation("DROP TABLE IF EXISTS weather_data");
            loader.connector.CloseSession();
        }
    }
}