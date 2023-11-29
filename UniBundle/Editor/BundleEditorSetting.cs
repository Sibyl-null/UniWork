using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace UniWork.UniBundle.Editor
{
    public class BundleEditorSetting : ScriptableObject
    {
        public string bundleFolder = "AssetBundle";
        
        // ----------------------------------------------

        public const string LoadPath = "Assets/Editor/Config/BundleEditorSetting.asset";
        
        public static BundleEditorSetting MustLoad()
        {
            BundleEditorSetting setting = AssetDatabase.LoadAssetAtPath<BundleEditorSetting>(LoadPath);
            if (setting == null)
                throw new BuildFailedException("[UniBundle] 加载 EditorSetting 失败");

            return setting;
        }
    }
}