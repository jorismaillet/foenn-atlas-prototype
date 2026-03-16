using System.Data;
using Assets.Scripts;
using Assets.Scripts.Database;
using Assets.Scripts.ETL.Loaders;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Engine;
using Mono.Data.Sqlite;
using NUnit.Framework;

namespace Assets.Editor.Tests.ETL
{
    public class LoaderTest
    {
        [Test]
        public void TestInsert()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            using (var connection = SqliteHelper.CreateConnection())
            {
                var dataset = WeatherHistoryDataset.Instance;
                dataset.InitTables(connection);
                SqliteHelper.Execute(connection, $"INSERT INTO \"{dataset.coreFact.Name}\" (temperature) VALUES (20);");
                var res = new QueryRequest(dataset.coreFact).Execute(connection);
                Assert.AreEqual(res.rows.Count, 1);
            }
        }

        [Test]
        public void TestStageLine_MapsCsvColumnsToExpectedSqlParameters()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            using (var connection = SqliteHelper.CreateConnection())
            {
                var dataset = WeatherHistoryDataset.Instance;
                var loader = new SqliteTableLoader(dataset.location);
                using (var transaction = connection.BeginTransaction())
                {
                    var csvHeaders = new[] { "NOM_USUEL", "ALTI", "LON", "NUM_POSTE", "LAT" };
                    loader.StartStaging(connection, transaction, csvHeaders);

                    var csvLine = new[] { "ANTIBES-GAROUPE", "75", "7.133000", "06004002", "43.564667" };
                    loader.StageLine(csvLine);

                    transaction.Commit();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT post_number, lat, lon, post_name, department, altitude FROM \"location_dimension_staging\"";

                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        Assert.AreEqual("06004002", reader.GetString(0));
                        Assert.AreEqual(43.564667d, reader.GetDouble(1), 0.0001d);
                        Assert.AreEqual(7.133000, reader.GetDouble(2), 0.0001d);
                        Assert.AreEqual("ANTIBES-GAROUPE", reader.GetString(3));
                        Assert.AreEqual("06", reader.GetString(4));
                        Assert.AreEqual(75, reader.GetInt64(5));
                        Assert.IsFalse(reader.Read());
                    }
                }

                loader.Dispose();
            }
        }
    }
}
