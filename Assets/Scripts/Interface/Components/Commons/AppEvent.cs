using System;
using System.Collections.Generic;

namespace Assets.Scripts.Interface.Components.Commons
{
    public class AppEvent<T>
    {
        public List<Action<T>> callBacks = new List<Action<T>>();

        public void AddListener(Action<T> callBack)
        {
            callBacks.Add(callBack);
        }

        public void RemoveListener(Action<T> callBack)
        {
            callBacks.Remove(callBack);
        }

        public void Invoke(T _)
        {
            var callBacksCache = callBacks.ToArray();

            foreach (Action<T> callBack in callBacksCache)
            {
                callBack.Invoke(_);
            }
        }
    }
}
