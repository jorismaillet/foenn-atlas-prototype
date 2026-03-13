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
            var fileName = "Tests/Weathers/H_06_truncated_test.csv";
            var extractor = new CSVExtractor(fileName);
            var headers = extractor.ExtractFieldNames();
            Assert.AreEqual(204, headers.Length);
            var lines = extractor.ExtractValues().ToList();
            Assert.AreEqual(26936, lines.Count);
            Assert.AreEqual(204, lines[0].Length);
            Assert.AreEqual("06004002", lines[0][0]);
        }
    }
}
