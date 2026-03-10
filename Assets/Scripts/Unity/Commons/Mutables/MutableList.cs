namespace Assets.Scripts.Unity.Commons.Mutables
{
    using System.Collections.Generic;

    public class MutableList<T> : List<T>
    {
        public GameEvent<List<T>> onChange = new GameEvent<List<T>>();

        public GameEvent<T> onElementAdded = new GameEvent<T>();

        public GameEvent<T> onElementRemoved = new GameEvent<T>();

        /*public List<T> Value {
            get { return this; }
        }*/
        public void Replace(List<T> newList)
        {
            Clear();
            AddRange(newList);
        }

        public new void Add(T element)
        {
            base.Add(element);
            onElementAdded.Invoke(element);
            onChange.Invoke(this);
        }

        public new void RemoveRange(int startIndex, int quantity)
        {
            base.RemoveRange(startIndex, quantity);
            onChange.Invoke(this);
        }

        public void AddRange(List<T> addedElements)
        {
            foreach (var el in addedElements)
            {
                Add(el);
            }
        }

        public new void Remove(T element)
        {
            base.Remove(element);
            onElementRemoved.Invoke(element);
            onChange.Invoke(this);
        }

        public new void Clear()
        {
            base.Clear();
            onChange.Invoke(this);
        }
    }
}
