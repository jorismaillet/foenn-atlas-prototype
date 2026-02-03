
using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Unity.Commons.Behaviours;
using Assets.Scripts.Unity.Sounds;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Common.Utils.RadioButton {
    public class RadioButtonSelector : BaseBehaviour {
        public RadioButtonGroup group;
        public List<GameObject> contents = new List<GameObject>();
        public Color selectedColor, unselectedColor;
        public InterfaceSoundKey sound;

        public static Dictionary<RadioButtonGroup, List<RadioButtonSelector>> radios = new Dictionary<RadioButtonGroup, List<RadioButtonSelector>>();

        protected virtual void Awake() {
            if (!radios.ContainsKey(group)) {
                radios.Add(group, new List<RadioButtonSelector>());
            }
            radios[group].Add(this);
            AddListener(GetComponent<Button>().onClick, Click);
        }

        protected override void OnDestroy() {
            radios[group].Remove(this);
            base.OnDestroy();
        }

        public virtual void Select() {
            Button button = GetComponent<Button>();
            SwitchColor(button.targetGraphic.GetComponent<Image>(), button.colors.fadeDuration, selectedColor);
            InterfaceAudioSource.Play(sound);
            foreach (GameObject content in contents) {
                content.SetActive(true);
            }
        }

        public virtual void UnSelect() {
            Button button = GetComponent<Button>();
            SwitchColor(button.targetGraphic.GetComponent<Image>(), button.colors.fadeDuration, unselectedColor);
            foreach (GameObject content in contents) {
                content.SetActive(false);
            }
        }

        public virtual void Click() {
            if (!group.Equals(RadioButtonGroup.NONE)) {
                foreach (RadioButtonSelector selector in radios[group].Without(this)) {
                    selector.UnSelect();
                }
            }
            Select();
        }
    }
}
