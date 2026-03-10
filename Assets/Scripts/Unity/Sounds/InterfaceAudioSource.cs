namespace Assets.Scripts.Unity.Sounds
{
    using System.Collections.Generic;
    using UnityEngine;

    public class InterfaceAudioSource : MonoBehaviour
    {
        private static AudioSource source;

        private static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

        private const string interfaceAudioClipsPrefix = "Sounds/Interface/{0}";

        private static Dictionary<InterfaceSoundKey, string> fileName = new Dictionary<InterfaceSoundKey, string>() {
            { InterfaceSoundKey.BUTTON, "button" },
            { InterfaceSoundKey.BUTTON2, "button2" },
            { InterfaceSoundKey.CHAT, "chat" },
            { InterfaceSoundKey.DRAG, "drag" },
            { InterfaceSoundKey.DROP, "drop" },
            { InterfaceSoundKey.MODAL, "modal" },
            { InterfaceSoundKey.MODAL_OPEN, "modal_open" },
            { InterfaceSoundKey.MODAL_CLOSE, "modal_close" },
            { InterfaceSoundKey.SETTINGS, "settings" },
            { InterfaceSoundKey.SETTINGS2, "settings2" },
            { InterfaceSoundKey.SETTINGS_CLOSE, "settings_close" },
            { InterfaceSoundKey.ERROR, "error" },
            { InterfaceSoundKey.START, "start" },
            { InterfaceSoundKey.TOGGLE, "toggle" },
            { InterfaceSoundKey.LOGIN, "login" },
            { InterfaceSoundKey.FIGHT, "fight" },
            { InterfaceSoundKey.FIGHT_LOSE, "fight_lose" },
            { InterfaceSoundKey.FIGHT_WIN, "fight_win" },
            { InterfaceSoundKey.MENU_OPEN, "menu_open" },
            { InterfaceSoundKey.MENU_CLOSE, "menu_close" },
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

        public static void Play(InterfaceSoundKey key)
        {
            source.PlayOneShot(GetAudioClip(fileName[key]));
        }
    }
}
