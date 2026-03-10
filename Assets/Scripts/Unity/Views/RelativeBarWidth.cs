namespace Assets.Scripts.Unity.Common.Views
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using Assets.Scripts.Unity.Commons.Mutables;
    using UnityEngine.UI;

    public class RelativeBarWidth : BaseBehaviour
    {
        public Image image;

        private Mutable<int> current, max;

        public void Initialize(Mutable<int> current, Mutable<int> max)
        {
        }
    }
}
