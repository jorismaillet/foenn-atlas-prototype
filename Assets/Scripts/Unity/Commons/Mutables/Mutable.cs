namespace Assets.Scripts.Unity.Commons.Mutables
{

    public class Mutable<T>
    {

        private T value;
        public GameEvent<T> onChange { get; } = new GameEvent<T>();

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

        public bool HasValue
        {
            get
            {
                return value != null;
            }
        }

        public void Set(T element)
        {
            Value = element;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            if (!HasValue)
            {
                return defaultValue;
            }
            return Value;
        }

        public void Clear()
        {
            Value = default(T);
        }

        public object GetValue() => value;
    }
}