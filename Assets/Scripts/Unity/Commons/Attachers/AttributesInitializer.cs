using Assets.Scripts.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Attachers {
    public class AttributesInitializer : MonoBehaviour {}
    [CustomEditor(typeof(AttributesInitializer))]
    public class AttributesInitializerEditor : Editor {
        public override void OnInspectorGUI() {
            if (GUILayout.Button("Initialize attributes", EditorStyles.miniButton)) {
                ((AttributesInitializer)target).GetComponentsInChildren<BaseHolder>().Each(holder => {
                    holder.onInitialize.Invoke();
                });
                EditorUtility.SetDirty(target);
            }
            DrawDefaultInspector();
        }
    }
}
