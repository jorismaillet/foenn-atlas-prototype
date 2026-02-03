using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Sounds
{
    public class ButtonSound : BaseBehaviour
    {
        public InterfaceSoundKey soundKey;

        private void Awake()
        {
            AddListener(GetComponent<Button>().onClick, PlaySound);
        }

        private void PlaySound()
        {
            InterfaceAudioSource.Play(soundKey);
        }
    }
}
