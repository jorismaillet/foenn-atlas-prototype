namespace Assets.Scripts.Unity.Sounds
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine.UI;

    public class ToggleSound : BaseBehaviour
    {
        public InterfaceSoundKey onSoundKey, offSoundKey;

        private void Awake()
        {
            AddListener(GetComponent<Toggle>().onValueChanged, PlaySound);
        }

        private void PlaySound(bool isOn)
        {
            if (isOn)
            {
                InterfaceAudioSource.Play(onSoundKey);
            }
            else
            {
                InterfaceAudioSource.Play(offSoundKey);
            }
        }
    }
}
