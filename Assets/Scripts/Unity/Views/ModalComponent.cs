namespace Assets.Scripts.Unity.Common.Views
{
    using Assets.Scripts.Unity.Common.Utils;
    using Assets.Scripts.Unity.Commons.Behaviours;
    using Assets.Scripts.Unity.Commons.Utils;
    using System;
    using UnityEngine.UI;

    public class ModalComponent : BaseBehaviour
    {
        public Text header, message;

        public Button confirmButton, cancelButton;

        private Action OnConfirm;

        public static void Show(string header, string message, Action OnConfirm = null)
        {
            PrefabUtil
                .AddGameObject("Common/Modal", "Modal", RootCanvas.instance.transform)
                .GetComponent<ModalComponent>()
                .Initialize(header, message, OnConfirm);
        }

        public void Initialize(string header, string message, Action OnConfirm)
        {
            this.header.text = header;
            this.message.text = message;
            this.OnConfirm = OnConfirm;
            AddListener(confirmButton.onClick, Confirm);
            AddListener(cancelButton.onClick, Cancel);
        }

        private void Confirm()
        {
            OnConfirm?.Invoke();
            Destroy(gameObject);
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }
    }
}
