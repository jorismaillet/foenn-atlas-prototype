using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components.Commons.Holders
{
    public class Holder<Element> : MonoBehaviour, IElementInitializer<Element>
    {
        public Element element;
        public UnityEvent onInitialize;

        public virtual void Initialize(Element element)
        {
            this.element = element;
            onInitialize.Invoke();
        }
    }
}
