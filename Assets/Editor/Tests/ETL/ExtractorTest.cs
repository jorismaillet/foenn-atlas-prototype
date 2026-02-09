using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.CSV;
using Assets.Scripts.Foenn.ETL.Extractors;
using NUnit.Framework;

namespace Assets.Editor.Tests.ETL
{
    public class ExtractorTest
    {
        [Test]
        public void TestCSVExtraction()
        {
            var fileName = "Tests/Weathers/H_29_latest-2023-2024";
            var dataset = new Dataset();
            new CSVExtractor(fileName).Extract(dataset);
            Assert.AreEqual(204, dataset.fields.Count);
            Assert.AreEqual(4, dataset.lines.Count);
            Assert.AreEqual(204, dataset.lines[0].Count);
        }
    }
}