namespace Assets.Scripts.Unity.Common.Utils.RadioButton
{
    using Assets.Scripts.Unity.Sounds;

    public class RadioToggleSelector : RadioButtonSelector
    {
        public bool isOn = false;

        public InterfaceSoundKey offSound;

        public override void Select()
        {
            if (isOn)
            {
                InterfaceAudioSource.Play(offSound);
                UnSelect();
            }
            else
            {
                base.Select();
                isOn = true;
            }
        }

        public override void UnSelect()
        {
            base.UnSelect();
            isOn = false;
        }

        private void OnDisable()
        {
            base.UnSelect();
        }

        private void OnEnable()
        {
            if (isOn)
            {
                base.Select();
            }
        }
    }
}
