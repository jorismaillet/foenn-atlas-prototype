namespace Assets.Scripts.Unity
{
    using System.Collections.Concurrent;
    using UnityEngine;

    public static class MainThreadLog
    {
        private static readonly ConcurrentQueue<string> _q = new();

        public static void Log(string msg) => _q.Enqueue(msg);

        // Appelée sur le main thread (Update)
        public static void Flush()
        {
            while (_q.TryDequeue(out var msg))
                Debug.Log(msg);
        }
    }
}
