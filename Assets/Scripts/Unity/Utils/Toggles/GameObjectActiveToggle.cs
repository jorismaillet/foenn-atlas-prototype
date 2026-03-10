namespace Assets.Scripts.Unity.Common.Utils.Toggles
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameObjectActiveToggle : BaseBehaviour
    {
        public GameObject targetGameObject;

        private void Awake()
        {
            AddListener(GetComponent<Toggle>().onValueChanged, OnClick);
        }

        public void OnClick(bool isOn)
        {
            targetGameObject.SetActive(isOn);
        }
    }
}
