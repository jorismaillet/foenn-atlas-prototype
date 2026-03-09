using Assets.Scripts.Foenn;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.Datasets;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.Utils;
using Mono.Data.Sqlite;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using UnityEditor.MemoryProfiler;

namespace Assets.Editor.Tests.ETL
{
    public class LoaderTest
    {
        [Test]
        public void TestInsert() {
            Env.SetDatabasePath(SqliteHelper.DATABASE_TEST_PATH);
            using (var connection = SqliteHelper.CreateConnection()) {
                var dataset = new WeatherHistoryDataset();
                dataset.InitTables(connection);
                var loader = new SqliteTableLoader(dataset.fact);
                SqliteHelper.Execute(connection, $"INSERT INTO \"{dataset.fact.Name}\" (ID) VALUES (1);");
                var res = new QueryRequest(dataset.fact.Name).Execute(connection);
                Assert.AreEqual(res.rows.Count, 1);
                CleanupDataset(connection, dataset);
            }
        }

        [Test]
        public void TestLoad() {

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