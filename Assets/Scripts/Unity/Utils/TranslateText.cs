using Assets.Scripts.App.Translations;
using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Common.Utils
{
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
