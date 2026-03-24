using System.Collections.Concurrent;
using UnityEngine;

namespace Assets.Scripts.Components.Logger
{
    public static class MainThreadLog
    {
        private static readonly ConcurrentQueue<string> _q = new();

        public static void Log(string msg) => _q.Enqueue(msg);

        public static void Flush()
        {
            while (_q.TryDequeue(out var msg))
                Debug.Log(msg);
        }
    }
}
