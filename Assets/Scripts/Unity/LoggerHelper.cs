namespace Assets.Scripts.Unity
{
    using UnityEngine;

    public class LoggerHelper : MonoBehaviour
    {
        void Update()
        {
            MainThreadLog.Flush();
        }
    }
}
