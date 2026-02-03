using Assets.Scripts.Unity.Commons.Behaviours;
using Assets.Scripts.Unity.Sounds;

namespace Assets.Scripts.Unity.Common.Actions
{
    public class SoundActions : BaseBehaviour
    {
        public void PlaySound(InterfaceSoundKey key)
        {
            InterfaceAudioSource.Play(key);
        }
    }
}
