using System;
using System.Collections.Generic;

namespace Assets.Scripts.Unity.Commons
{
    public class GameEvent
    {
        public List<Action> callBacks = new List<Action>();

        public void AddListener(Action callBack)
        {
            callBacks.Add(callBack);
        }

        public void RemoveListener(Action callBack)
        {
            callBacks.Remove(callBack);
        }

        public void Invoke()
        {
            foreach (Action callBack in callBacks)
            {
                callBack.Invoke();
            }
        }
    }

    public class GameEvent<T>
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

    public class GameEvent<T1, T2>
    {
        public List<Action<T1, T2>> callBacks = new List<Action<T1, T2>>();

        public void AddListener(Action<T1, T2> callBack)
        {
            callBacks.Add(callBack);
        }

        public void RemoveListener(Action<T1, T2> callBack)
        {
            callBacks.Remove(callBack);
        }

        public void Invoke(T1 _1, T2 _2)
        {
            foreach (Action<T1, T2> callBack in callBacks)
            {
                callBack.Invoke(_1, _2);
            }
        }
    }

    public class GameEvent<T1, T2, T3>
    {
        public List<Action<T1, T2, T3>> callBacks = new List<Action<T1, T2, T3>>();

        public void AddListener(Action<T1, T2, T3> callBack)
        {
            callBacks.Add(callBack);
        }

        public void RemoveListener(Action<T1, T2, T3> callBack)
        {
            callBacks.Remove(callBack);
        }

        public void Invoke(T1 _1, T2 _2, T3 _3)
        {
            foreach (Action<T1, T2, T3> callBack in callBacks)
            {
                callBack.Invoke(_1, _2, _3);
            }
        }
    }
}