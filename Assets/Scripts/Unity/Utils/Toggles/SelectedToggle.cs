using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Common.Utils.Toggles
{
    public class SelectedToggle : BaseBehaviour
    {
        public Color selectedColor;
        private Toggle toggle;
        private Image toggleImage;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggleImage = toggle.targetGraphic.GetComponent<Image>();
            AddListener(toggle.onValueChanged, (selected) =>
            {
                if (selected)
                {
                    Select();
                }
                else
                {
                    Unselect();
                }
            });
        }

        public void Select()
        {
            SwitchColor(toggleImage, toggle.colors.fadeDuration, selectedColor);
        }

        public void Unselect()
        {
            SwitchColor(toggleImage, toggle.colors.fadeDuration, toggle.colors.normalColor);
        }
    }
}