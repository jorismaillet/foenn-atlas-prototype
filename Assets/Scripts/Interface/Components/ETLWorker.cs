using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.ETL;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interface.Components.Logger;
using Assets.Scripts.OLAP.Datasets;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class ETLWorker : MonoBehaviour
    {
        private CancellationTokenSource ct;

        private Task task;

        private void Start()
        {
            SqliteHelper.CreateDb();

            var dataset = WeatherHistoryDataset.Instance;
            List<string> filesToLoad;

            using (var sqliteConnection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.Instance.InitTables(sqliteConnection);
                SqliteHelper.CreateTable(sqliteConnection, dataset.MetadataTable);
                filesToLoad = dataset.MetadataTable.FilesToLoad(sqliteConnection);
            }

            StartCoroutine(Init(dataset, filesToLoad));
        }

        IEnumerator Init(Dataset dataset, List<string> filesToLoad)
        {
            if (filesToLoad.Any())
            {
                Application.runInBackground = true;
                Application.targetFrameRate = 1;
                yield return StartCoroutine(PrepareData(dataset, filesToLoad));
                Application.runInBackground = false;
                Application.targetFrameRate = 60;
            }
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
            var processor = new ETLProcessor(fileName, dataset);
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

        void OnDisable()
        {
            CleanupRunningWork();
        }

        void OnApplicationQuit()
        {
            CleanupRunningWork();
        }
    }
}
