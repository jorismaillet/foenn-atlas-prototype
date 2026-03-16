using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Components.Logger;
using Assets.Scripts.Database;
using Assets.Scripts.ETL;
using Assets.Scripts.OLAP.Datasets;
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

        public IEnumerator PrepareData(Dataset dataset, List<string> filesToLoad)
        {
            var sw = new Stopwatch();
            sw.Start();
            MainThreadLog.Log("Initialize data");
            foreach (var fileName in filesToLoad)
            {
                MainThreadLog.Log($"Check {fileName}");
                yield return LoadFile(dataset, fileName);
            }
            sw.Stop();
            MainThreadLog.Log($"Data prepared in {sw.Elapsed}s");
        }

        public IEnumerator LoadFile(Dataset dataset, string fileName)
        {
            var processor = new ETLProcessor(
                    fileName,
                    dataset.Dimensions,
                    dataset.Facts
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
                    dataset.MetadataTable.FlagProcessed(connection, fileName);
                }
            }
            CleanupRunningWork();
        }
    }
}
