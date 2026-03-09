using Assets.Scripts.Foenn;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.Datasets;
using Assets.Scripts.Foenn.ETL.Datasources;
using Mono.Data.Sqlite;
using NUnit.Framework;
using UnityEditor.MemoryProfiler;

namespace Assets.Editor.Tests.ETL
{
    public class ETLProcessorTest
    {
        [Test]
        public void TestETLProcessor() {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            using (var connection = SqliteHelper.CreateConnection()) {
                var dataset = new WeatherHistoryDataset();
                dataset.InitTables(connection);
                var fileName = "Tests/Weathers/H_29_latest-2023-2024.csv";
                var processor = new WeatherHistoryProcessor(fileName, dataset);
                processor.ProcessETL();

                var res = new QueryRequest(dataset.fact.Name).Execute(connection);
                Assert.AreEqual(res.rows.Count, 4);

                CleanupDataset(connection, dataset);
            }
        }

        private void CleanupDataset(SqliteConnection connection, WeatherHistoryDataset dataset) {
            foreach (var table in dataset.Tables) {
                SqliteHelper.DropStagingTable(connection, table);
                SqliteHelper.Execute(connection, $"DROP TABLE IF EXISTS {table.Name}");
                SqliteHelper.Execute(connection, $"DROP TABLE IF EXISTS {MetadataTable.TableName(table.Name)}");
            }
        }
    }
}
