using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Utils
{
    public class PrefabUtil : MonoBehaviour
    {
        public static GameObject AddGameObject(string prefabPath, string name = null, Transform parent = null)
        {
            GameObject gameObject = LoadPrefab(prefabPath, name);
            if (parent != null)
            {
                gameObject.transform.SetParent(parent, false);
            }
            return gameObject;
        }

        public static void DestroyChildrenGameObjects<T>(Transform transform) where T : MonoBehaviour
        {
            T[] array = transform.GetComponentsInChildren<T>().Cast<T>().ToArray();
            for (int i = array.Length - 1; i >= 0; i--)
            {
                T component = array[i];
                if (component.transform != transform)
                {
                    Destroy(component.transform.gameObject);
                }
            }
        }

        public static void DestroyAllChildren(Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        public static void DestroyImmediateAllChildren(Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }

        private static GameObject LoadPrefab(string path, string name = null)
        {
            try
            {
                GameObject prefab = (GameObject)Instantiate(Resources.Load("Prefabs/" + path));
                if (name != null)
                {
                    prefab.name = name;
                }
                return prefab;
            }
            catch
            {
                throw new Exception("Error: Cannot load prefab 'Prefabs / " + path + "'");
            }
        }
    }
}
