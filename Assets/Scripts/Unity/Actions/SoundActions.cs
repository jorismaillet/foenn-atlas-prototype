namespace Assets.Scripts.Unity.Common.Actions
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using Assets.Scripts.Unity.Sounds;

    public class SoundActions : BaseBehaviour
    {
        public void PlaySound(InterfaceSoundKey key)
        {
            InterfaceAudioSource.Play(key);
        }
    }
}
