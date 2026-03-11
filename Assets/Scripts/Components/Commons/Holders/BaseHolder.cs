using System.Collections.Generic;
using Assets.Scripts.Components.Commons.Attachers;
using Assets.Scripts.Components.Commons.Behaviours;
using UnityEngine.Events;

namespace Assets.Scripts.Components.Commons.Holders
{
    public abstract class BaseHolder : BaseBehaviour
    {
        protected List<BaseAttacher> attachers = new List<BaseAttacher>();

        public UnityEvent onInitialize;

        public virtual void Attach(BaseAttacher attacher)
        {
            attachers.Add(attacher);
        }
    }
}
