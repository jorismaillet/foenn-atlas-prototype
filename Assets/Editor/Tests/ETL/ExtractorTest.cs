using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Models;
using NUnit.Framework;
using System.Linq;

namespace Assets.Editor.Tests.ETL
{
    public class ExtractorTest
    {
        [Test]
        public void TestCSVExtraction()
        {
            var fileName = "Tests/Weathers/H_29_latest-2023-2024.csv";
            var schema = new SchemaDefinition("weather_data");
            var extractor = new CSVExtractor(fileName);
            var headers = extractor.ExtractHeaders();
            Assert.AreEqual(204, headers.Count);
            var lines = extractor.ExtractContent(headers.Count).ToList();
            Assert.AreEqual(4, lines.Count);
            Assert.AreEqual(204, lines[0].Length);
        }
    }
}