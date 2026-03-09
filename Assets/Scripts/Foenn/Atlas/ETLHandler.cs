using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
using static UnityEngine.Mesh;

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
            if (DatabaseHelper.connection == null) return;

            try { DatabaseHelper.connection.Close(); } catch { }
            try { DatabaseHelper.connection.Dispose(); } catch { }
            DatabaseHelper.connection = null;
        }

        public IEnumerator PrepareData(List<string> filesToLoad)
        {
            var sw = new Stopwatch();
            sw.Start();
            MainThreadLog.Log("Initialize data");
            var metadata = new MetadataDatasource(WeatherHistoryDatasource.tableName);
            foreach (var fileName in filesToLoad)
            {
                MainThreadLog.Log($"Check {fileName}");
                string dpt = fileName.Split('_')[1];
                var datasource = new WeatherHistoryDatasource(dpt);
                yield return LoadFile(datasource, fileName, metadata);
            }
            sw.Stop();
            MainThreadLog.Log($"Data prepared in {sw.ElapsedMilliseconds}ms");
        }

        public IEnumerator LoadFile(Datasource datasource, string file, MetadataDatasource metadata)
        {
            var processor = new ETLProcessor(
                    datasource,
                    new CSVExtractor(file),
                    new SqliteLoader(datasource)
                );
            ct = new CancellationTokenSource();
            task = Task.Run(() => processor.ProcessETL(ct.Token), ct.Token);

            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                UnityEngine.Debug.LogException(task.Exception);
            metadata.FlagProcessed(file);
            CleanupRunningWork();
        }
    }
}
