using UnityEngine;

namespace Assets.Scripts.Unity
{
    public class LoggerHelper : MonoBehaviour
    {
        void Update()
        {
            MainThreadLog.Flush();
        }
    }
}