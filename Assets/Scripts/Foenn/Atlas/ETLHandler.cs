using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.Datasets;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Unity;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

            if (task != null)
            {
                try { task.Wait(2000); }
                catch { }
            }

            task = null;

            ct?.Dispose();
            ct = null;
        }

        public IEnumerator PrepareData(SqliteConnection connection, List<string> filesToLoad, MetadataTable metadata)
        {
            var sw = new Stopwatch();
            sw.Start();
            MainThreadLog.Log("Initialize data");
            foreach (var fileName in filesToLoad)
            {
                MainThreadLog.Log($"Check {fileName}");
                string dpt = fileName.Split('_')[1];
                yield return LoadFile(connection, fileName, metadata);
            }
            sw.Stop();
            MainThreadLog.Log($"Data prepared in {sw.ElapsedMilliseconds}ms");
        }

        public IEnumerator LoadFile(SqliteConnection connection,  string fileName, MetadataTable metadata)
        {
            var processor = new WeatherHistoryProcessor(
                    fileName
                );
            ct = new CancellationTokenSource();
            task = Task.Run(() => processor.ProcessETL(ct.Token), ct.Token);

            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                UnityEngine.Debug.LogException(task.Exception);
            metadata.FlagProcessed(connection, fileName);
            CleanupRunningWork();
        }
    }
}
