using Assets.Scripts.Components.Commons.Attachers;

namespace Assets.Scripts.Components.Commons.Holders
{
    public abstract class Holder<Element> : BaseHolder, IElementInitializer<Element>
    {
        public Element element;

        private bool set = false;

        public override void Attach(BaseAttacher attacher)
        {
            base.Attach(attacher);
            if (set)
            {
                attacher.Set(element);
            }
        }

        public virtual void Initialize(Element element)
        {
            this.element = element;
            set = true;
            ReInit();
        }

        public virtual void ReInit()
        {
            ClearListeners();
            attachers.ForEach(attacher =>
            {
                attacher.Set(element);
            });
            if (element != null)
            {
                onInitialize.Invoke();
            }
        }
    }
}
