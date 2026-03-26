using System.Collections.Generic;

namespace Assets.Scripts.Interface.Components.Commons
{
    public class MutableList<T> : List<T>
    {
        public AppEvent<List<T>> onChange = new AppEvent<List<T>>();

        public new void Add(T element)
        {
            base.Add(element);
            onChange.Invoke(this);
        }

        public void AddRange(List<T> addedElements)
        {
            base.AddRange(addedElements);
            onChange.Invoke(this);
        }

        public new void Clear()
        {
            base.Clear();
            onChange.Invoke(this);
        }
    }
}
