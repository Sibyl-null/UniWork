using UnityEditor;
using UniWork.UniBundle.Editor.SettingDefine;
using UniWork.Utility.Editor;

namespace UniWork.UniBundle.Editor
{
    internal static class BundleMenuItem
    {
        [MenuItem("UniWork/UniBundle/创建 EditorSetting")]
        private static void CreateEditorSetting()
        {
            EditorMethodUtility.CreateScriptableObjectAsset<BundleEditorSetting>(BundleEditorSetting.LoadPath);
        }
    }
}