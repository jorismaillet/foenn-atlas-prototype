using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Components.Logger;
using Assets.Scripts.Database;
using Assets.Scripts.ETL;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class ETLWorker : MonoBehaviour
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
                task.Dispose();
            }

            task = null;

            ct?.Dispose();
            ct = null;
        }

        public IEnumerator PrepareData(List<string> filesToLoad, MetadataTable metadata)
        {
            var sw = new Stopwatch();
            sw.Start();
            MainThreadLog.Log("Initialize data");
            foreach (var fileName in filesToLoad)
            {
                MainThreadLog.Log($"Check {fileName}");
                string dpt = fileName.Split('_')[1];
                yield return LoadFile(fileName, metadata);
            }
            sw.Stop();
            MainThreadLog.Log($"Data prepared in {sw.ElapsedMilliseconds}ms");
        }

        public IEnumerator LoadFile(string fileName, MetadataTable metadata)
        {
            var processor = new ETLProcessor(
                    fileName,
                    WeatherHistoryDataset.Dimensions,
                    WeatherHistoryDataset.Facts
                );
            ct = new CancellationTokenSource();
            task = Task.Run(() =>
            {
                using (var connection = SqliteHelper.CreateConnection())
                {
                    processor.ProcessETL(connection, ct.Token);
                }
            }, ct.Token);

            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                UnityEngine.Debug.LogException(task.Exception);
            else
            {
                using (var connection = SqliteHelper.CreateConnection())
                {
                    metadata.FlagProcessed(connection, fileName);
                }
            }
            CleanupRunningWork();
        }
    }
}
