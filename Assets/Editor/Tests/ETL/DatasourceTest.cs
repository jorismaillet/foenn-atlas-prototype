using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.ETL.Transformers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;

namespace Assets.Editor.Tests.ETL
{
    public class DatasourceTest
    {
        [Test]
        public void TestIdentifier()
        {
            var datasource = new WeatherHistoryDatasource();
            Dictionary<string, int> headerIndexes = new Dictionary<string, int> { { "NUM_POSTE", 0 }, { "AAAAMMJJHH", 1 } };
            string[] line = new string[] { "290001", "2023010112" };
            Assert.AreEqual(datasource.Identifier(headerIndexes, line), "2900012023010112");
        }

        [Test]
        public void TestTransform()
        {
            var datasource = new WeatherHistoryDatasource();
            var schema = new SchemaDefinition(datasource.TableName());
            var transformer = new Transformer(datasource);
            schema.AddColumns(new List<Datafield> { new Datafield("NUM_POSTE", DbType.String), new Datafield("AAAAMMJJHH", DbType.String) });
            datasource.PrepareTranformer(schema);
            transformer.TransformColumns(schema);
            string[] line = new string[] { "290001", "2023010112" };
            new Transformer(datasource).TransformLine(schema, line);
            var extraColumns = datasource.GetExtraColumns(line);
            Assert.AreEqual(schema.columns.Count, 3);
            Assert.AreEqual(schema.columns[0].name, "NUM_POSTE");
            Assert.AreEqual(schema.columns[0].type, DbType.String);
            Assert.AreEqual(schema.columns[1].name, "AAAAMMJJHH");
            Assert.AreEqual(schema.columns[1].type, DbType.String);
            Assert.AreEqual(schema.columns[2].name, "ID");
            Assert.AreEqual(schema.columns[2].type, DbType.String);
            Assert.AreEqual(line.Length, 2);
            Assert.AreEqual(line[0], "290001");
            Assert.AreEqual(line[1], "2023010112");
            Assert.AreEqual(extraColumns.Length, 1);
            Assert.AreEqual(extraColumns[0], "2900012023010112");
        }
    }
}