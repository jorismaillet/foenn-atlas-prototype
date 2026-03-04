using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Transformers;
using Assets.Scripts.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas
{
    public class ETLHandler : MonoBehaviour
    {
        private CancellationTokenSource ct;
        private Task task;

        void OnDisable()
        {
            CleanupRunningWork();
        }

        void OnApplicationQuit()
        {
            CleanupRunningWork();
        }

        void CleanupRunningWork()
        {
            try { ct?.Cancel(); } catch { }

            // Best-effort: wait a bit so the background ETL thread can hit cancellation
            // and run its `finally` blocks (where it closes the SQLite session).
            if (task != null)
            {
                try { task.Wait(2000); }
                catch { /* ignore */ }
            }

            task = null;

            ct?.Dispose();
            ct = null;

            ForceCloseSqlConnection();
        }

        static void ForceCloseSqlConnection()
        {
            if (SqlConnector.connection == null) return;

            try { SqlConnector.connection.Close(); } catch { }
            try { SqlConnector.connection.Dispose(); } catch { }
            SqlConnector.connection = null;
        }

        public IEnumerator PrepareData()
        {
            MainThreadLog.Log("Initialize data");

            var weathersDir = Path.Combine(UnityEngine.Application.dataPath, "Resources", "Weathers");
            if (!Directory.Exists(weathersDir))
            {
                MainThreadLog.Log($"Weathers folder not found: {weathersDir}");
                yield break;
            }

            var files = Directory.EnumerateFiles(weathersDir, "*.csv", SearchOption.TopDirectoryOnly)
                .Select(f => Path.Combine("Weathers", Path.GetFileName(f)))
                .ToList();

            foreach (var fileName in files)
            {
                MainThreadLog.Log(fileName);
                var datasource = new WeatherHistoryDatasource();
                yield return LoadFile(datasource, fileName);
            }
            MainThreadLog.Log("Done");
        }

        public IEnumerator LoadFile(Datasource datasource, string file)
        {
            // Ensure we don't keep an old ETL task (and its SQLite lock) alive.
            CleanupRunningWork();

            var processor = new ETLProcessor(
                    datasource,
                    new CSVExtractor(file),
                    new Transformer(datasource),
                    new SqliteLoader(datasource)
                );
            if (!processor.ShouldProcess())
                yield break;
            ct = new CancellationTokenSource();
            task = Task.Run(() => processor.ProcessETL(ct.Token), ct.Token);

            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                UnityEngine.Debug.LogException(task.Exception);

            // Make sure the DB connection is not left open/locked between files.
            CleanupRunningWork();
        }
    }
}
