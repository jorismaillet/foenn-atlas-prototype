using Assets.Scripts.Components.Commons.Behaviours;
using Assets.Scripts.Components.Commons.Holders;
using UnityEngine.Events;

namespace Assets.Scripts.Components.Commons.Attachers
{
    public abstract class BaseAttacher : BaseBehaviour
    {
        public UnityEvent onInitialize;

        public BaseHolder holder;

        protected virtual void Awake()
        {
            if (holder == null)
            {
                throw new System.Exception($"{GetType().Name} ({name}): Holder is null");
            }
            holder.Attach(this);
        }

        public abstract void Set<T>(T element);
    }
}
