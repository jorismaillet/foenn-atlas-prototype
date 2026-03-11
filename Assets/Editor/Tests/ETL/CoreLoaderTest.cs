namespace Assets.Editor.Tests.ETL
{
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.ETL;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class SqliteHelperTest
    {
        [Test]
        public void FieldToSql_StringType_ReturnsText()
        {
            var field = Field.Text("name");
            var sql = SqliteHelper.FieldToSql(field);
            Assert.AreEqual("TEXT", sql);
        }

        [Test]
        public void FieldToSql_DoubleType_ReturnsReal()
        {
            var field = Field.Metric("temperature");
            var sql = SqliteHelper.FieldToSql(field);
            Assert.AreEqual("REAL", sql);
        }

        [Test]
        public void FieldToSql_IntTypes_ReturnInteger()
        {
            Assert.AreEqual("INTEGER", SqliteHelper.FieldToSql(Field.Int("count")));
            Assert.AreEqual("INTEGER", SqliteHelper.FieldToSql(Field.Int16("year")));
            Assert.AreEqual("INTEGER", SqliteHelper.FieldToSql(Field.Int64("timestamp")));
        }

        [Test]
        public void FieldToSql_PrimaryKey_AddsPrimaryKeyClause()
        {
            var field = Field.PK();
            var sql = SqliteHelper.FieldToSql(field);
            Assert.AreEqual("INTEGER PRIMARY KEY AUTOINCREMENT", sql);
        }

        [Test]
        public void FieldToSql_PrimaryKey_SkipPK_NoPrimaryKeyClause()
        {
            var field = Field.PK();
            var sql = SqliteHelper.FieldToSql(field, skipPK: true);
            Assert.AreEqual("INTEGER", sql);
        }
    }

    public class SqliteTableLoaderTest
    {
        [Test]
        public void FindCsvIndex_ExistingColumn_ReturnsIndex()
        {
            var loader = new TestableLoader(new TestTable());
            var csvFieldNames = new[] { "COL_A", "COL_B", "COL_C" };
            
            var index = loader.TestFindCsvIndex("COL_B", csvFieldNames);
            Assert.AreEqual(1, index);
        }

        [Test]
        public void FindCsvIndex_CaseInsensitive()
        {
            var loader = new TestableLoader(new TestTable());
            var csvFieldNames = new[] { "COL_A", "Col_B", "COL_C" };
            
            var index = loader.TestFindCsvIndex("col_b", csvFieldNames);
            Assert.AreEqual(1, index);
        }

        [Test]
        public void FindCsvIndex_NotFound_ReturnsMinusOne()
        {
            var loader = new TestableLoader(new TestTable());
            var csvFieldNames = new[] { "COL_A", "COL_B" };
            
            var index = loader.TestFindCsvIndex("COL_Z", csvFieldNames);
            Assert.AreEqual(-1, index);
        }

        [Test]
        public void GetConverter_String_ReturnsStringOrNull()
        {
            var loader = new TestableLoader(new TestTable());
            var converter = loader.TestGetConverter(DbType.String);
            
            Assert.AreEqual("hello", converter("hello"));
            Assert.AreEqual(DBNull.Value, converter(""));
        }

        [Test]
        public void GetConverter_Double_ParsesDouble()
        {
            var loader = new TestableLoader(new TestTable());
            var converter = loader.TestGetConverter(DbType.Double);
            
            Assert.AreEqual(123.45, converter("123.45"));
            Assert.AreEqual(DBNull.Value, converter(""));
        }

        [Test]
        public void GetConverter_Int32_ParsesInt()
        {
            var loader = new TestableLoader(new TestTable());
            var converter = loader.TestGetConverter(DbType.Int32);
            
            Assert.AreEqual(42, converter("42"));
        }

        private class TestableLoader : SqliteTableLoader
        {
            public TestableLoader(ITable table) : base(table) { }
            
            public int TestFindCsvIndex(string name, string[] fields) => FindCsvIndex(name, fields);
            public Func<string, object> TestGetConverter(DbType type) => GetConverter(type);
        }

        private class TestTable : ITable
        {
            public string TableName => "test";
            public Field PrimaryKey => Field.PK();
            public List<IndexDefinition> Indexes => new List<IndexDefinition>();
            public List<FieldMap> Mappings => new List<FieldMap>()
            {
                FieldMap.MapText("NAME", "name"),
                FieldMap.Map("VALUE", "value")
            };
        }
    }

    public class DimensionCacheTest
    {
        [Test]
        public void TryGetId_EmptyCache_ReturnsFalse()
        {
            var dimension = new TestDimension();
            var cache = new DimensionCache(dimension);
            
            var found = cache.TryGetId("test", out int id);
            
            Assert.IsFalse(found);
            Assert.AreEqual(0, id);
        }

        [Test]
        public void Count_EmptyCache_ReturnsZero()
        {
            var dimension = new TestDimension();
            var cache = new DimensionCache(dimension);
            
            Assert.AreEqual(0, cache.Count);
        }

        private class TestDimension : IDimension
        {
            public string TableName => "test_dim";
            public Field PrimaryKey => Field.PK();
            public Field LookupKey => Field.Text("code");
            public List<IndexDefinition> Indexes => new List<IndexDefinition>();
            public List<FieldMap> Mappings => new List<FieldMap>()
            {
                FieldMap.MapText("CODE", "code")
            };
        }
    }
}
