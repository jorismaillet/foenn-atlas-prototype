using System;
using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Attachers
{
    public abstract class Attacher<Element> : BaseAttacher, IElementInitializer<Element> where Element : class
    {
        public Element element;


        public override void Set<BaseElement>(BaseElement element)
        {
            try
            {
                this.element = (element as Element);
                if (element != null && this.element == null)
                {
                    throw new InvalidCastException($"Elements are not compatible! {typeof(Element)} & {element.GetType()}");
                }
                if (this.element != null)
                {
                    Initialize(this.element);
                }
            }
            catch (InvalidCastException e)
            {
                Debug.LogError($"{name} contains {GetType().Name}. Holder link mismatch. Expects {typeof(Element)} but is {element.GetType()}", gameObject);
                throw e;
            }
        }

        public virtual void Initialize(Element element)
        {
            onInitialize.Invoke();
        }
    }
}
