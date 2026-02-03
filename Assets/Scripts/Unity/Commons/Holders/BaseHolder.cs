using Assets.Scripts.Unity.Commons.Attachers;
using Assets.Scripts.Unity.Commons.Behaviours;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Assets.Scripts.Unity.Commons.Holders {
    public abstract class BaseHolder : BaseBehaviour {
        protected List<BaseAttacher> attachers = new List<BaseAttacher>();

        public UnityEvent onInitialize;

        public virtual void Attach(BaseAttacher attacher) {
            attachers.Add(attacher);
        }
    }
}
