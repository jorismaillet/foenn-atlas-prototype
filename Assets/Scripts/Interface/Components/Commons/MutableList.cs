using System.Collections.Generic;

namespace Assets.Scripts.Components.Commons.Mutables
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
            foreach (var el in addedElements)
            {
                Add(el);
            }
        }

        public new void Clear()
        {
            base.Clear();
            onChange.Invoke(this);
        }
    }
}
