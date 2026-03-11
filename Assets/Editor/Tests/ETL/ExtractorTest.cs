using System.Linq;
using Assets.Scripts.ETL.Extractors;
using NUnit.Framework;

namespace Assets.Editor.Tests.ETL
{
    public class ExtractorTest
    {
        [Test]
        public void TestCSVExtraction()
        {
            var fileName = "Tests/Weathers/H_29_latest-2023-2024.csv";
            var extractor = new CSVExtractor(fileName);
            var headers = extractor.ExtractFieldNames();
            Assert.AreEqual(204, headers.Length);
            var lines = extractor.ExtractValues().ToList();
            Assert.AreEqual(4, lines.Count);
            Assert.AreEqual(204, lines[0].Length);
        }
    }
}
