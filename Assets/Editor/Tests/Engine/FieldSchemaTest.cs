namespace Assets.Editor.Tests.Engine
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using NUnit.Framework;
    using System.Data;

    public class FieldTest
    {
        [Test]
        public void Metric_CreatesDoubleField()
        {
            var field = Field.Metric("temperature");
            
            Assert.AreEqual("temperature", field.name);
            Assert.AreEqual(DbType.Double, field.dbType);
            Assert.AreEqual(AnalyticsType.METRIC, field.analyticsType);
        }

        [Test]
        public void Text_CreatesStringField()
        {
            var field = Field.Text("name");
            
            Assert.AreEqual("name", field.name);
            Assert.AreEqual(DbType.String, field.dbType);
            Assert.AreEqual(AnalyticsType.ATTRIBUTE, field.analyticsType);
        }

        [Test]
        public void Int_CreatesInt32Field()
        {
            var field = Field.Int("count");
            Assert.AreEqual(DbType.Int32, field.dbType);
        }

        [Test]
        public void Int16_CreatesInt16Field()
        {
            var field = Field.Int16("year");
            Assert.AreEqual(DbType.Int16, field.dbType);
        }

        [Test]
        public void Int64_CreatesInt64Field()
        {
            var field = Field.Int64("timestamp");
            Assert.AreEqual(DbType.Int64, field.dbType);
        }

        [Test]
        public void PK_CreatesPrimaryKeyField()
        {
            var field = Field.PK();
            
            Assert.AreEqual("ID", field.name);
            Assert.IsTrue(field.isPrimaryKey);
            Assert.IsTrue(field.autoIncrement);
        }

        [Test]
        public void PK_WithCustomName()
        {
            var field = Field.PK("custom_id");
            
            Assert.AreEqual("custom_id", field.name);
            Assert.IsTrue(field.isPrimaryKey);
        }

        [Test]
        public void ToSql_SimpleField()
        {
            var field = Field.Metric("temperature");
            Assert.AreEqual("\"temperature\"", field.ToSql());
        }

        [Test]
        public void As_AddsAggregation()
        {
            var field = Field.Metric("temperature").As(AggregationKey.AVG);
            
            Assert.AreEqual(AggregationKey.AVG, field.aggregation);
            Assert.AreEqual("AVG(\"temperature\")", field.ToSql());
        }

        [Test]
        public void IsReference_FalseForRegularField()
        {
            var field = Field.Metric("temperature");
            Assert.IsFalse(field.IsReference);
        }
    }

    public class FieldMappingTest
    {
        [Test]
        public void Map_WithField_CreatesDirectMapping()
        {
            var field = Field.Metric("temperature");
            var mapping = FieldMap.Map("T", field);
            
            Assert.AreEqual("T", mapping.csvColumn);
            Assert.AreEqual(field, mapping.targetField);
            Assert.IsNull(mapping.transform);
        }

        [Test]
        public void Map_WithFieldName_CreatesMetricField()
        {
            var mapping = FieldMap.Map("T", "temperature");
            
            Assert.AreEqual("T", mapping.csvColumn);
            Assert.AreEqual("temperature", mapping.targetField.name);
            Assert.AreEqual(DbType.Double, mapping.targetField.dbType);
        }

        [Test]
        public void MapText_CreatesTextField()
        {
            var mapping = FieldMap.MapText("NAME", "post_name");
            
            Assert.AreEqual("NAME", mapping.csvColumn);
            Assert.AreEqual("post_name", mapping.targetField.name);
            Assert.AreEqual(DbType.String, mapping.targetField.dbType);
        }

        [Test]
        public void MapInt_CreatesIntField()
        {
            var mapping = FieldMap.MapInt("COUNT", "item_count");
            
            Assert.AreEqual("COUNT", mapping.csvColumn);
            Assert.AreEqual(DbType.Int32, mapping.targetField.dbType);
        }

        [Test]
        public void Compute_WithTransform_AppliesTransform()
        {
            var field = Field.Int16("year");
            var mapping = FieldMap.Compute("DATE", field, s => int.Parse(s.Substring(0, 4)));
            
            Assert.AreEqual("DATE", mapping.csvColumn);
            Assert.AreEqual(field, mapping.targetField);
            Assert.IsNotNull(mapping.transform);
            
            var result = mapping.transform("20231215");
            Assert.AreEqual(2023, result);
        }
    }

    public class IndexDefinitionTest
    {
        [Test]
        public void Constructor_UniqueIndex_SetsProperties()
        {
            var field1 = Field.Int("col1");
            var field2 = Field.Int("col2");
            var index = new IndexDefinition(true, field1, field2);
            
            Assert.IsTrue(index.unique);
            Assert.AreEqual(2, index.fields.Count);
        }

        [Test]
        public void Name_GeneratesCorrectFormat()
        {
            var field1 = Field.Int("time_id");
            var field2 = Field.Int("location_id");
            var index = new IndexDefinition(true, field1, field2);
            
            Assert.AreEqual("idx_time_id_location_id", index.name);
        }
    }
}
