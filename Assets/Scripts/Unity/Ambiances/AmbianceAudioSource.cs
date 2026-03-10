namespace Assets.Scripts.Unity.Ambiances
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AmbianceAudioSource : MonoBehaviour
    {
        private static AudioSource source;

        private static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

        private const string interfaceAudioClipsPrefix = "Sounds/Ambiance/{0}";

        private static Dictionary<AmbianceSoundKey, string> fileName = new Dictionary<AmbianceSoundKey, string>()
        {

        };

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private static AudioClip GetAudioClip(string fileName)
        {
            string path = string.Format(interfaceAudioClipsPrefix, fileName);
            if (!clips.ContainsKey(path))
            {
                AudioClip clip = Resources.Load<AudioClip>(path);
                clips.Add(path, clip);
            }
            return clips[path];
        }

        public static void Play(AmbianceSoundKey key)
        {
            source.PlayOneShot(GetAudioClip(fileName[key]));
        }
    }
}
