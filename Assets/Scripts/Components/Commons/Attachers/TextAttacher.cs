namespace Assets.Scripts.Components.Commons.Attachers
{
    public abstract class TextAttacher<Element> : Attacher<Element> where Element : class
    {
        public override void Initialize(Element element)
        {
            SetText(Text(element));
        }

        public abstract string Text(Element element);
    }
}
