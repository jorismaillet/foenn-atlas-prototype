using UnityEngine;

namespace Assets.Scripts.Interface.Components.Logger
{
    public class LoggerHelper : MonoBehaviour
    {
        void Update()
        {
            MainThreadLog.Flush();
        }
    }
}
