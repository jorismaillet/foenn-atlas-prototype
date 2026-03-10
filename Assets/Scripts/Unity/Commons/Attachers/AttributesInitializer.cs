namespace Assets.Scripts.Unity.Commons.Attachers
{
    using Assets.Scripts.Common.Extensions;
    using Assets.Scripts.Unity.Commons.Holders;
    using UnityEditor;
    using UnityEngine;

    public class AttributesInitializer : MonoBehaviour
    {
    }

    [CustomEditor(typeof(AttributesInitializer))]
    public class AttributesInitializerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Initialize attributes", EditorStyles.miniButton))
            {
                ((AttributesInitializer)target).GetComponentsInChildren<BaseHolder>().Each(holder =>
                {
                    holder.onInitialize.Invoke();
                });
                EditorUtility.SetDirty(target);
            }
            DrawDefaultInspector();
        }
    }
}
