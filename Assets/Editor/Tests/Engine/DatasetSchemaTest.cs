namespace Assets.Editor.Tests.Engine
{
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using NUnit.Framework;
    using System.Linq;

    public class TimeDimensionTest
    {
        private TimeDimension _dimension;

        [SetUp]
        public void Setup()
        {
            _dimension = new TimeDimension();
        }

        [Test]
        public void TableName_IsTime()
        {
            Assert.AreEqual("time", _dimension.TableName);
        }

        [Test]
        public void PrimaryKey_IsAutoIncrement()
        {
            Assert.IsTrue(_dimension.PrimaryKey.isPrimaryKey);
            Assert.IsTrue(_dimension.PrimaryKey.autoIncrement);
        }

        [Test]
        public void Mappings_YearTransform_ExtractsYear()
        {
            var yearMapping = _dimension.Mappings.First(m => m.targetField.name == "year");
            var result = yearMapping.transform("2023121508");
            Assert.AreEqual(2023, result);
        }

        [Test]
        public void Mappings_MonthTransform_ExtractsMonth()
        {
            var monthMapping = _dimension.Mappings.First(m => m.targetField.name == "month");
            var result = monthMapping.transform("2023121508");
            Assert.AreEqual(12, result);
        }

        [Test]
        public void Mappings_DayTransform_ExtractsDay()
        {
            var dayMapping = _dimension.Mappings.First(m => m.targetField.name == "day");
            var result = dayMapping.transform("2023121508");
            Assert.AreEqual(15, result);
        }

        [Test]
        public void Mappings_HourTransform_ExtractsHour()
        {
            var hourMapping = _dimension.Mappings.First(m => m.targetField.name == "hour");
            var result = hourMapping.transform("2023121508");
            Assert.AreEqual(8, result);
        }
    }

    public class LocationDimensionTest
    {
        private LocationDimension _dimension;

        [SetUp]
        public void Setup()
        {
            _dimension = new LocationDimension();
        }

        [Test]
        public void TableName_IsLocation()
        {
            Assert.AreEqual("location", _dimension.TableName);
        }

        [Test]
        public void Mappings_DepartmentTransform_ExtractsFirst2Chars()
        {
            var deptMapping = _dimension.Mappings.First(m => m.targetField.name == "department");
            var result = deptMapping.transform("29001");
            Assert.AreEqual("29", result);
        }
    }

    public class WeatherFactSchemaTest
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
        public void References_ContainsTimeAndLocation()
        {
            Assert.AreEqual(2, _fact.References.Count);
            
            var timeRef = _fact.References.First(r => r.name == "time_id");
            var locRef = _fact.References.First(r => r.name == "location_id");
            
            Assert.IsTrue(timeRef.IsReference);
            Assert.IsTrue(locRef.IsReference);
        }

        [Test]
        public void TimeRef_ReferencesTimeDimension()
        {
            Assert.AreEqual(_time, _fact.timeRef.referencedDimension);
            Assert.AreEqual("AAAAMMJJHH", _fact.timeRef.sourceCsvColumn);
        }

        [Test]
        public void LocationRef_ReferencesLocationDimension()
        {
            Assert.AreEqual(_location, _fact.locationRef.referencedDimension);
            Assert.AreEqual("NUM_POSTE", _fact.locationRef.sourceCsvColumn);
        }

        [Test]
        public void Mappings_ContainsTemperatureAndRain()
        {
            var fieldNames = _fact.Mappings.Select(m => m.targetField.name).ToList();
            
            Assert.Contains("temperature", fieldNames);
            Assert.Contains("rain_1", fieldNames);
        }
    }
}
