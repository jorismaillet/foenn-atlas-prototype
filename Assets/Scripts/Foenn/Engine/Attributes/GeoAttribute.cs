using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Attributes
{
    public class GeoAttribute : ScriptableObject
    {
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }
    }
}