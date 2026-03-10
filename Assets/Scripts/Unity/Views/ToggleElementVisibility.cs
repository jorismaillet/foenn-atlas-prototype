namespace Assets.Scripts.Unity.Common.Views
{
    using Assets.Scripts.Unity.Commons.Attachers;

    public class ToggleElementVisibility : BaseAttacher
    {
        public override void Set<T>(T element)
        {
            gameObject.SetActive(!Equals(element, default(T)));
        }
    }
}
