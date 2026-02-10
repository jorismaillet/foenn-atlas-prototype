using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.CSV;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Transformers;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Editor.Tests.ETL
{
    public class DatasourceTest
    {
        [Test]
        public void TestIdentifier()
        {
            var datasource = new WeatherHistoryDatasource();
            Dictionary<string, int> headerIndexes = new Dictionary<string, int> { { "NUM_POSTE", 0 }, { "AAAAMMJJHH", 1 } };
            List<string> line = new List<string> { "290001", "2023010112" };
            Assert.AreEqual(datasource.Identifier(headerIndexes, line), "2900012023010112");
        }

        [Test]
        public void TestTransform()
        {
            var datasource = new WeatherHistoryDatasource();
            var dataset = new Dataset();
            dataset.fields = new List<Datafield> { new Datafield("NUM_POSTE", Datatype.INT), new Datafield("AAAAMMJJHH", Datatype.STRING) };
            dataset.lines = new List<List<string>> { new List<string> { "290001", "2023010112" } };
            new Transformer(datasource).Transform(dataset);
            Assert.AreEqual(dataset.fields.Count, 4);
            Assert.AreEqual(dataset.fields[0].name, "ID");
            Assert.AreEqual(dataset.fields[0].type, Datatype.PRIMARY_KEY);
            Assert.AreEqual(dataset.fields[1].name, "INSERT_ID");
            Assert.AreEqual(dataset.fields[1].type, Datatype.STRING);
            Assert.AreEqual(dataset.fields[2].name, "NUM_POSTE");
            Assert.AreEqual(dataset.fields[2].type, Datatype.INT);
            Assert.AreEqual(dataset.fields[3].name, "AAAAMMJJHH");
            Assert.AreEqual(dataset.fields[3].type, Datatype.STRING);
            Assert.AreEqual(dataset.lines.Count, 1);
            Assert.AreEqual(dataset.lines[0].Count, 4);
            Assert.AreEqual(dataset.lines[0][0], "");
            Assert.AreEqual(dataset.lines[0][1], "2900012023010112");
            Assert.AreEqual(dataset.lines[0][2], "290001");
            Assert.AreEqual(dataset.lines[0][3], "2023010112");
        }
    }
}