using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Models;
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
            var schema = new SchemaDefinition(datasource.TableName());
            var transformer = new Transformer(datasource);
            schema.AddHeaders(new List<Datafield> { new Datafield("NUM_POSTE", Datatype.INT), new Datafield("AAAAMMJJHH", Datatype.STRING) });
            transformer.TransformHeaders(schema);
            var line = new List<string> { "290001", "2023010112" };
            new Transformer(datasource).TransformLine(schema, line);
            Assert.AreEqual(schema.headers.Count, 4);
            Assert.AreEqual(schema.headers[0].name, "ID");
            Assert.AreEqual(schema.headers[0].type, Datatype.PRIMARY_KEY);
            Assert.AreEqual(schema.headers[1].name, "INSERT_ID");
            Assert.AreEqual(schema.headers[1].type, Datatype.STRING);
            Assert.AreEqual(schema.headers[2].name, "NUM_POSTE");
            Assert.AreEqual(schema.headers[2].type, Datatype.INT);
            Assert.AreEqual(schema.headers[3].name, "AAAAMMJJHH");
            Assert.AreEqual(schema.headers[3].type, Datatype.STRING);
            Assert.AreEqual(line.Count, 4);
            Assert.AreEqual(line[0], "");
            Assert.AreEqual(line[1], "2900012023010112");
            Assert.AreEqual(line[2], "290001");
            Assert.AreEqual(line[3], "2023010112");
        }
    }
}