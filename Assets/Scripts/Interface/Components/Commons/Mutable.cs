namespace Assets.Scripts.Components.Commons.Mutables
{
    public class Mutable<T>
    {
        private T value;

        public AppEvent<T> onChange { get; } = new AppEvent<T>();

        public Mutable(T t = default(T))
        {
            Value = t;
        }

        public static implicit operator Mutable<T>(T value)
        {
            Mutable<T> result = new Mutable<T>(value);
            return result;
        }

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                onChange.Invoke(value);
            }
        }

        public void Set(T element)
        {
            Value = element;
        }
    }
}
