namespace Assets.Scripts.Unity.Common.Utils
{
    using Assets.Scripts.App.Translations;
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine.UI;

    public class TranslateText : BaseBehaviour
    {
        public UIStringId key;

        private void Start()
        {
            OnMutation(Locale.language, _ =>
            {
                GetComponent<Text>().text = Locale.Get(key);
            });
        }
    }
}
