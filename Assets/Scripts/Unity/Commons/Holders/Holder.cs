namespace Assets.Scripts.Unity.Commons.Holders
{
    using Assets.Scripts.Unity.Commons.Attachers;

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
