namespace Assets.Editor.Tests.Engine
{
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Assets.Scripts.Foenn.OLAP.Sql;
    using NUnit.Framework;

    public class QueryRequestTest
    {
        private WeatherFact _fact;
        private TimeDimension _time;
        private LocationDimension _location;

        [SetUp]
        public void Setup()
        {
            _time = new TimeDimension();
            _location = new LocationDimension();
            _fact = new WeatherFact(_time, _location);
        }

        [Test]
        public void Select_SingleField_AddsToSelectedColumns()
        {
            var query = new QueryRequest(_fact)
                .Select(WeatherFact.temperature);

            Assert.AreEqual(1, query.selectedColumns.Count);
        }

        [Test]
        public void Select_WithAggregation_CreatesAggregatedField()
        {
            var query = new QueryRequest(_fact)
                .Select(AggregationKey.AVG, WeatherFact.temperature);

            var field = query.selectedColumns[0] as Field;
            Assert.AreEqual(AggregationKey.AVG, field.aggregation);
        }

        [Test]
        public void Select_WithTableAndField_CreatesPrefixedField()
        {
            var query = new QueryRequest(_fact)
                .Select(_location, LocationDimension.PostName);

            var field = query.selectedColumns[0] as Field;
            Assert.AreEqual(_location, field.table);
        }

        [Test]
        public void GroupBy_AddsFieldsToGroups()
        {
            var query = new QueryRequest(_fact)
                .GroupBy(LocationDimension.PostName, TimeDimension.year);

            Assert.AreEqual(2, query.groups.Count);
        }

        [Test]
        public void Where_AddsFilter()
        {
            var query = new QueryRequest(_fact)
                .Where(new DataFilter(
                    LocationDimension.Department.Of(_location), 
                    DataFilterMode.INCLUDE, 
                    "29"));

            Assert.AreEqual(1, query.filters.Count);
        }

        [Test]
        public void Join_AddsJoinDefinition()
        {
            var query = new QueryRequest(_fact)
                .Join(_fact, _fact.locationRef, JoinType.INNER);

            Assert.AreEqual(1, query.joins.Count);
        }

        [Test]
        public void ToSql_GeneratesValidSql()
        {
            var query = new QueryRequest(_fact)
                .Select(WeatherFact.temperature.As(AggregationKey.AVG))
                .Select(_location, LocationDimension.Department)
                .GroupBy(LocationDimension.Department);

            var sql = query.ToSql();

            Assert.IsTrue(sql.Contains("SELECT"));
            Assert.IsTrue(sql.Contains("AVG"));
            Assert.IsTrue(sql.Contains("GROUP BY"));
        }

        [Test]
        public void ChainedMethods_BuildsCompleteQuery()
        {
            var query = new QueryRequest(_fact)
                .Select(WeatherFact.temperature.Of(_fact, AggregationKey.AVG))
                .Select(_location, LocationDimension.PostName)
                .Join(_fact, _fact.locationRef, JoinType.INNER)
                .Join(_fact, _fact.timeRef, JoinType.INNER)
                .Where(new DataFilter(LocationDimension.Department.Of(_location), DataFilterMode.INCLUDE, "29"))
                .GroupBy(LocationDimension.PostName);

            Assert.AreEqual(2, query.selectedColumns.Count);
            Assert.AreEqual(2, query.joins.Count);
            Assert.AreEqual(1, query.filters.Count);
            Assert.AreEqual(1, query.groups.Count);
        }
    }
}
