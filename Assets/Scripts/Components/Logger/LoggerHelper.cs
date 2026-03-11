using UnityEngine;

namespace Assets.Scripts.Components.Logger
{
    public class LoggerHelper : MonoBehaviour
    {
        void Update()
        {
            MainThreadLog.Flush();
        }
    }
}
