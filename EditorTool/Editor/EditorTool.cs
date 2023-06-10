using UnityEditor;
using UnityEngine;

namespace SFramework.EditorTool.Editor
{
    public static class EditorTool
    {
        [MenuItem("SFramework/EditorTool/打开PersistentDataPath")]
        public static void Open()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}