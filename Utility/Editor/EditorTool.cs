using UnityEditor;
using UnityEngine;

namespace UniWork.Utility.Editor
{
    public static class EditorTool
    {
        [MenuItem("UniWork/EditorTool/打开PersistentDataPath")]
        public static void Open()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}